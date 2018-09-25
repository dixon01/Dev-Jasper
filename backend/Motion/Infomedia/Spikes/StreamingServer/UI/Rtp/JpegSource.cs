namespace Gorba.Motion.Infomedia.Spike.StreamingServer.UI.Rtp
{
    using System;
    using System.Collections.Generic;
    using System.Drawing;
    using System.Drawing.Imaging;
    using System.Globalization;
    using System.IO;

    public class JpegSource
    {
        private readonly Random random = new Random();
        private readonly uint SourceId;
        private int sequenceNumber;

        private int timestamp = 48298449;

        public JpegSource()
        {
            this.SourceId = (uint)this.random.Next();
            this.sequenceNumber = this.random.Next(0, 0x10000);
        }

        public IEnumerable<Buffer> GetPackets()
        {
            int size;
            var data = CreateJpeg(out size);
            int offset = 0;

            GstRtpJPEGPay pay = new GstRtpJPEGPay();
            CompInfo[] infos = null;
            RtpQuantTable[] tables = new RtpQuantTable[15];

            /* parse the jpeg header for 'start of scan' and read quant tables if needed */
            bool sos_found = false;
            bool dqt_found = false;
            bool sof_found = false;
            bool dri_found = false;
            int jpeg_header_size = 0;

            while (!sos_found && (offset < size))
            {
                ////GST_LOG_OBJECT(pay, "checking from offset %u", offset);
                var marker = gst_rtp_jpeg_pay_scan_marker(data, size, ref offset);
                switch (marker)
                {
                    case RtpJpegMarker.JPEG_MARKER_JFIF:
                    case RtpJpegMarker.JPEG_MARKER_CMT:
                    case RtpJpegMarker.JPEG_MARKER_DHT:
                    case RtpJpegMarker.JPEG_MARKER_H264:
                        Console.WriteLine("skipping marker: " + marker);
                        offset += gst_rtp_jpeg_pay_header_size(data, offset);
                        break;
                    case RtpJpegMarker.JPEG_MARKER_SOF:
                        infos = gst_rtp_jpeg_pay_read_sof(pay, data, size, ref offset);
                        sof_found = true;
                        break;
                    case RtpJpegMarker.JPEG_MARKER_DQT:
                        Console.WriteLine("DQT found");
                        gst_rtp_jpeg_pay_read_quant_table(tables, data, size, ref offset);
                        dqt_found = true;
                        break;
                    case RtpJpegMarker.JPEG_MARKER_SOS:
                        sos_found = true;
                        Console.WriteLine("SOS found");
                        jpeg_header_size = offset + gst_rtp_jpeg_pay_header_size(data, offset);
                        break;
                    case RtpJpegMarker.JPEG_MARKER_EOI:
                        Console.WriteLine("EOI reached before SOS!");
                        break;
                    case RtpJpegMarker.JPEG_MARKER_SOI:
                        Console.WriteLine("SOI found");
                        break;
                    case RtpJpegMarker.JPEG_MARKER_DRI:
                        ////GST_LOG_OBJECT(pay, "DRI found");
                        /*if (gst_rtp_jpeg_pay_read_dri(pay, data, size, &offset,
                                &restart_marker_header))*/
                        dri_found = true;
                        break;
                }
            }

            if (!dqt_found || !sof_found)
            {
                throw new InvalidDataException("Unsupported JPEG");
            }

            /* by now we should either have negotiated the width/height or the SOF header
             * should have filled us in */
            if (pay.width == 0 || pay.height == 0)
            {
                throw new InvalidDataException("No dimensions defined!");
            }

            Console.WriteLine("Header size {0}", jpeg_header_size);

            size -= jpeg_header_size;
            offset = 0;

            if (dri_found)
                pay.type += 64;

            /* prepare stuff for the jpeg header */
            var jpeg_header = new RtpJpegHeader();
            jpeg_header.type_spec = 0;
            jpeg_header.type = pay.type;
            jpeg_header.q = pay.quant;
            jpeg_header.width = (byte)pay.width;
            jpeg_header.height = (byte)pay.height;


            /* collect the quant headers sizes */
            var quant_header = new RtpQuantHeader();
            quant_header.mbz = 0;
            quant_header.precision = 0;
            quant_header.length = 0;
            int quant_data_size = 0;

            if (pay.quant > 127)
            {
                int tableCount = 0;
                foreach (var table in tables)
                {
                    if (table != null)
                    {
                        tableCount++;
                    }
                }

                /* for the Y and U component, look up the quant table and its size. quant
                 * tables for U and V should be the same */
                for (int i = 0; i < 2; i++)
                {
                    int qsize;
                    int qt;

                    qt = infos[i].qt;
                    if (qt >= tableCount)
                    {
                        throw new InvalidDataException("Invalid quant table index: " + qt);
                    }

                    qsize = tables[qt].Size;
                    if (qsize == 0)
                    {
                        throw new InvalidDataException("Empty quant table size at index " + qt);
                    }

                    quant_header.precision |= (byte)(qsize == 64 ? 0 : (1 << i));
                    quant_data_size += qsize;
                }

                quant_header.length = (ushort)quant_data_size;
                quant_data_size += 4; // 4 = sizeof(quant_header)
            }

            Console.WriteLine("quant_data size {0}", quant_data_size);

            int bytes_left = 8 + quant_data_size + size; // 8 = sizeof(jpeg_header)

            ////if (dri_found)
            ////    bytes_left += sizeof(restart_marker_header);

            const int mtu = 1438; // todo: check if this is right
            byte[] rtp = new byte[mtu + 12];
            bool frame_done = false;
            do
            {
                int payload_size = (bytes_left < mtu ? bytes_left : mtu);
                int header_size;

                header_size = 8 + quant_data_size; // 8 = sizeof(jpeg_header)
                ////if (dri_found)
                ////    header_size += sizeof(restart_marker_header);

                ////outbuf = gst_rtp_buffer_new_allocate(header_size, 0, 0);

                ////gst_rtp_buffer_map(outbuf, GST_MAP_WRITE, &rtp);

                rtp[0] = 1 << 7; // version 2 (RFC1889) 
                rtp[1] = 26; // JPEG
                rtp[2] = (byte)(this.sequenceNumber >> 8);
                rtp[3] = (byte)this.sequenceNumber;

                rtp[4] = (byte)(this.timestamp >> 24);
                rtp[5] = (byte)(this.timestamp >> 16);
                rtp[6] = (byte)(this.timestamp >> 8);
                rtp[7] = (byte)this.timestamp;

                rtp[8] = (byte)(this.SourceId >> 24);
                rtp[9] = (byte)(this.SourceId >> 16);
                rtp[10] = (byte)(this.SourceId >> 8);
                rtp[11] = (byte)this.SourceId;

                this.sequenceNumber++;

                if (payload_size == bytes_left)
                {
                    Console.WriteLine("last packet of frame");
                    frame_done = true;
                    rtp[1] |= 1 << 7; // marker bit
                }

                ////payload = gst_rtp_buffer_get_payload(&rtp);

                /* update offset */
                jpeg_header.offset = offset;

                int payloadOffset = 12;
                jpeg_header.CopyTo(rtp, payloadOffset);
                payload_size -= 8; // 8 = sizeof(jpeg_header)
                payloadOffset += 8;

                ////if (dri_found)
                ////{
                ////    memcpy(payload, &restart_marker_header, sizeof(restart_marker_header));
                ////    payload += sizeof(restart_marker_header);
                ////    payload_size -= sizeof(restart_marker_header);
                ////}

                /* only send quant table with first packet */
                if (quant_data_size > 0)
                {
                    payloadOffset += quant_header.CopyTo(rtp, payloadOffset);

                    /* copy the quant tables for luma and chrominance */
                    for (int i = 0; i < 2; i++)
                    {
                        int qsize;
                        int qt;

                        qt = infos[i].qt;
                        qsize = tables[qt].Size;
                        Array.Copy(tables[qt].Data, 0, rtp, payloadOffset, qsize);

                        Console.WriteLine("Component {0} using quant {1}, size {2}", i, qt, qsize);

                        payloadOffset += qsize;
                    }

                    payload_size -= quant_data_size;
                    bytes_left -= quant_data_size;
                    quant_data_size = 0;
                }

                Console.WriteLine("Sending payload size {0}", payload_size);

                /////* create a new buf to hold the payload */
                ////paybuf = gst_buffer_copy_region(buffer, GST_BUFFER_COPY_MEMORY,
                ////    jpeg_header_size + offset, payload_size);
                Array.Copy(data, jpeg_header_size + offset, rtp, payloadOffset, payload_size);
                payloadOffset += payload_size;

                yield return new Buffer(rtp, 0, payloadOffset);

                bytes_left -= payload_size;
                offset += payload_size;
                ////data += payload_size;
            }
            while (!frame_done);

            this.timestamp += 11250; // todo: what is the right increment for the timestamp?

            ////if (pay->buffer_list)
            ////{
            ////    /* push the whole buffer list at once */
            ////    ret = gst_rtp_base_payload_push_list(basepayload, list);
            ////}

            ////gst_buffer_unmap(buffer, &map);
            ////gst_buffer_unref(buffer);

            ////return ret;

////  GstRtpJPEGPay *pay;
////  GstClockTime timestamp;
////  GstFlowReturn ret = GST_FLOW_ERROR;
////  RtpJpegHeader jpeg_header;
////  RtpQuantHeader quant_header;
////  RtpRestartMarkerHeader restart_marker_header;
////  RtpQuantTable tables[15] = { {0, NULL}, };
////  CompInfo info[3] = { {0,}, };
////  guint quant_data_size;
////  GstMapInfo map;
////  guint8 *data;
////  gsize size;
////  guint mtu;
////  guint bytes_left;
////  guint jpeg_header_size = 0;
////  guint offset;
////  gboolean frame_done;
////  gboolean sos_found, sof_found, dqt_found, dri_found;
////  gint i;
////  GstBufferList *list = NULL;

////  pay = GST_RTP_JPEG_PAY (basepayload);
////  mtu = GST_RTP_BASE_PAYLOAD_MTU (pay);

////  gst_buffer_map (buffer, &map, GST_MAP_READ);
////  data = map.data;
////  size = map.size;
////  timestamp = GST_BUFFER_TIMESTAMP (buffer);
////  offset = 0;

////  GST_LOG_OBJECT (pay, "got buffer size %" G_GSIZE_FORMAT
////      " , timestamp %" GST_TIME_FORMAT, size, GST_TIME_ARGS (timestamp));

////  /* parse the jpeg header for 'start of scan' and read quant tables if needed */
////  sos_found = FALSE;
////  dqt_found = FALSE;
////  sof_found = FALSE;
////  dri_found = FALSE;

////  while (!sos_found && (offset < size)) {
////    GST_LOG_OBJECT (pay, "checking from offset %u", offset);
////    switch (gst_rtp_jpeg_pay_scan_marker (data, size, &offset)) {
////      case JPEG_MARKER_JFIF:
////      case JPEG_MARKER_CMT:
////      case JPEG_MARKER_DHT:
////      case JPEG_MARKER_H264:
////        GST_LOG_OBJECT (pay, "skipping marker");
////        offset += gst_rtp_jpeg_pay_header_size (data, offset);
////        break;
////      case JPEG_MARKER_SOF:
////        if (!gst_rtp_jpeg_pay_read_sof (pay, data, size, &offset, info))
////          goto invalid_format;
////        sof_found = TRUE;
////        break;
////      case JPEG_MARKER_DQT:
////        GST_LOG ("DQT found");
////        offset = gst_rtp_jpeg_pay_read_quant_table (data, size, offset, tables);
////        dqt_found = TRUE;
////        break;
////      case JPEG_MARKER_SOS:
////        sos_found = TRUE;
////        GST_LOG_OBJECT (pay, "SOS found");
////        jpeg_header_size = offset + gst_rtp_jpeg_pay_header_size (data, offset);
////        break;
////      case JPEG_MARKER_EOI:
////        GST_WARNING_OBJECT (pay, "EOI reached before SOS!");
////        break;
////      case JPEG_MARKER_SOI:
////        GST_LOG_OBJECT (pay, "SOI found");
////        break;
////      case JPEG_MARKER_DRI:
////        GST_LOG_OBJECT (pay, "DRI found");
////        if (gst_rtp_jpeg_pay_read_dri (pay, data, size, &offset,
////                &restart_marker_header))
////          dri_found = TRUE;
////        break;
////      default:
////        break;
////    }
////  }
////  if (!dqt_found || !sof_found)
////    goto unsupported_jpeg;

////  /* by now we should either have negotiated the width/height or the SOF header
////   * should have filled us in */
////  if (pay->width == 0 || pay->height == 0)
////    goto no_dimension;

////  GST_LOG_OBJECT (pay, "header size %u", jpeg_header_size);

////  size -= jpeg_header_size;
////  data += jpeg_header_size;
////  offset = 0;

////  if (dri_found)
////    pay->type += 64;

////  /* prepare stuff for the jpeg header */
////  jpeg_header.type_spec = 0;
////  jpeg_header.type = pay->type;
////  jpeg_header.q = pay->quant;
////  jpeg_header.width = pay->width;
////  jpeg_header.height = pay->height;

////  /* collect the quant headers sizes */
////  quant_header.mbz = 0;
////  quant_header.precision = 0;
////  quant_header.length = 0;
////  quant_data_size = 0;

////  if (pay->quant > 127) {
////    /* for the Y and U component, look up the quant table and its size. quant
////     * tables for U and V should be the same */
////    for (i = 0; i < 2; i++) {
////      guint qsize;
////      guint qt;

////      qt = info[i].qt;
////      if (qt >= G_N_ELEMENTS (tables))
////        goto invalid_quant;

////      qsize = tables[qt].size;
////      if (qsize == 0)
////        goto invalid_quant;

////      quant_header.precision |= (qsize == 64 ? 0 : (1 << i));
////      quant_data_size += qsize;
////    }
////    quant_header.length = g_htons (quant_data_size);
////    quant_data_size += sizeof (quant_header);
////  }

////  GST_LOG_OBJECT (pay, "quant_data size %u", quant_data_size);

////  if (pay->buffer_list) {
////    list = gst_buffer_list_new ();
////  }

////  bytes_left = sizeof (jpeg_header) + quant_data_size + size;

////  if (dri_found)
////    bytes_left += sizeof (restart_marker_header);

////  frame_done = FALSE;
////  do {
////    GstBuffer *outbuf;
////    guint8 *payload;
////    guint payload_size = (bytes_left < mtu ? bytes_left : mtu);
////    guint header_size;
////    GstBuffer *paybuf;
////    GstRTPBuffer rtp = { NULL };

////    header_size = sizeof (jpeg_header) + quant_data_size;
////    if (dri_found)
////      header_size += sizeof (restart_marker_header);

////    outbuf = gst_rtp_buffer_new_allocate (header_size, 0, 0);

////    gst_rtp_buffer_map (outbuf, GST_MAP_WRITE, &rtp);

////    if (payload_size == bytes_left) {
////      GST_LOG_OBJECT (pay, "last packet of frame");
////      frame_done = TRUE;
////      gst_rtp_buffer_set_marker (&rtp, 1);
////    }

////    payload = gst_rtp_buffer_get_payload (&rtp);

////    /* update offset */
////#if (G_BYTE_ORDER == G_LITTLE_ENDIAN)
////    jpeg_header.offset = ((offset & 0x0000FF) << 16) |
////        ((offset & 0xFF0000) >> 16) | (offset & 0x00FF00);
////#else
////    jpeg_header.offset = offset;
////#endif
////    memcpy (payload, &jpeg_header, sizeof (jpeg_header));
////    payload += sizeof (jpeg_header);
////    payload_size -= sizeof (jpeg_header);

////    if (dri_found) {
////      memcpy (payload, &restart_marker_header, sizeof (restart_marker_header));
////      payload += sizeof (restart_marker_header);
////      payload_size -= sizeof (restart_marker_header);
////    }

////    /* only send quant table with first packet */
////    if (G_UNLIKELY (quant_data_size > 0)) {
////      memcpy (payload, &quant_header, sizeof (quant_header));
////      payload += sizeof (quant_header);

////      /* copy the quant tables for luma and chrominance */
////      for (i = 0; i < 2; i++) {
////        guint qsize;
////        guint qt;

////        qt = info[i].qt;
////        qsize = tables[qt].size;
////        memcpy (payload, tables[qt].data, qsize);

////        GST_LOG_OBJECT (pay, "component %d using quant %d, size %d", i, qt,
////            qsize);

////        payload += qsize;
////      }
////      payload_size -= quant_data_size;
////      bytes_left -= quant_data_size;
////      quant_data_size = 0;
////    }
////    GST_LOG_OBJECT (pay, "sending payload size %d", payload_size);
////    gst_rtp_buffer_unmap (&rtp);

////    /* create a new buf to hold the payload */
////    paybuf = gst_buffer_copy_region (buffer, GST_BUFFER_COPY_MEMORY,
////        jpeg_header_size + offset, payload_size);

////    /* join memory parts */
////    outbuf = gst_buffer_join (outbuf, paybuf);

////    GST_BUFFER_TIMESTAMP (outbuf) = timestamp;

////    if (pay->buffer_list) {
////      /* and add to list */
////      gst_buffer_list_insert (list, -1, outbuf);
////    } else {
////      ret = gst_rtp_base_payload_push (basepayload, outbuf);
////      if (ret != GST_FLOW_OK)
////        break;
////    }

////    bytes_left -= payload_size;
////    offset += payload_size;
////    data += payload_size;
////  }
////  while (!frame_done);

////  if (pay->buffer_list) {
////    /* push the whole buffer list at once */
////    ret = gst_rtp_base_payload_push_list (basepayload, list);
////  }

////  gst_buffer_unmap (buffer, &map);
////  gst_buffer_unref (buffer);

////  return ret;

////  /* ERRORS */
////unsupported_jpeg:
////  {
////    GST_ELEMENT_ERROR (pay, STREAM, FORMAT, ("Unsupported JPEG"), (NULL));
////    gst_buffer_unmap (buffer, &map);
////    gst_buffer_unref (buffer);
////    return GST_FLOW_NOT_SUPPORTED;
////  }
////no_dimension:
////  {
////    GST_ELEMENT_ERROR (pay, STREAM, FORMAT, ("No size given"), (NULL));
////    gst_buffer_unmap (buffer, &map);
////    gst_buffer_unref (buffer);
////    return GST_FLOW_NOT_NEGOTIATED;
////  }
////invalid_format:
////  {
////    /* error was posted */
////    gst_buffer_unmap (buffer, &map);
////    gst_buffer_unref (buffer);
////    return GST_FLOW_ERROR;
////  }
////invalid_quant:
////  {
////    GST_ELEMENT_ERROR (pay, STREAM, FORMAT, ("Invalid quant tables"), (NULL));
////    gst_buffer_unmap (buffer, &map);
////    gst_buffer_unref (buffer);
////    return GST_FLOW_ERROR;
////  }
            yield break;
        }

        private static RtpJpegMarker gst_rtp_jpeg_pay_scan_marker(byte[] data, int size, ref int offset)
        {
            const int JPEG_MARKER = 0xFF;
            while (data[offset++] != JPEG_MARKER && offset < size)
            {
            }

            if (offset >= size)
            {
                return RtpJpegMarker.JPEG_MARKER_EOI;
            }

            byte marker = data[offset];
            ////GST_LOG ("found 0x%02x marker at offset %u", marker, offset);
            offset++;
            return (RtpJpegMarker)marker;
        }

        private static int gst_rtp_jpeg_pay_header_size(byte[] data, int offset)
        {
            return data[offset] << 8 | data[offset + 1];
        }

        private static CompInfo[] gst_rtp_jpeg_pay_read_sof(GstRtpJPEGPay pay, byte[] data, int size, ref int offset)
        {
            var infos = new CompInfo[3];

            int off = offset;

            /* we need at least 17 bytes for the SOF */
            if (off + 17 > size)
            {
                throw new InvalidDataException("SOF needs at least 17 bytes");
            }

            int sof_size = gst_rtp_jpeg_pay_header_size(data, off);
            if (sof_size < 17)
            {
                throw new InvalidDataException("Bad SOF size: " + sof_size);
            }

            offset += sof_size;

            /* skip size */
            off += 2;

            /* precision should be 8 */
            int precision = data[off++];
            if (precision != 8)
            {
                throw new InvalidDataException("Unsupported precision: " + precision);
            }

            /* read dimensions */
            int height = data[off] << 8 | data[off + 1];
            int width = data[off + 2] << 8 | data[off + 3];
            off += 4;

            Console.WriteLine("Got dimensions {0}x{1}", height, width);

            if (height == 0 || height > 2040)
            {
                throw new InvalidDataException("Height outside bounds: " + height);
            }

            if (width == 0 || width > 2040)
            {
                throw new InvalidDataException("Width outside bounds: " + width);
            }

            pay.height = (height + 7) / 8; // round up division by 8
            pay.width = (width + 7) / 8; // round up division by 8

            /* we only support 3 components */
            int components = data[off++];
            if (components != 3)
            {
                throw new InvalidDataException("Only 3 components supporte, but got: " + components);
            }

            for (int i = 0; i < 3; i++)
            {
                infos[i] = new CompInfo();
                infos[i].id = data[off++];
                infos[i].samp = data[off++];
                infos[i].qt = data[off++];
                Console.WriteLine("Got comp {0}, samp {1:X2}, qt {2}", infos[i].id, infos[i].samp, infos[i].qt);
            }

            /* sort from the last element to the first */
            Array.Sort(infos, (left, right) => left.id - right.id);

            /* see that the components are supported */
            if (infos[0].samp == 0x21)
            {
                pay.type = 0;
            }
            else if (infos[0].samp == 0x22)
            {
                pay.type = 1;
            }
            else
            {
                throw new InvalidDataException("Invalid 1st Component: " + infos[0].samp);
            }

            if (infos[1].samp != 0x11)
            {
                throw new InvalidDataException("Invalid 2nd Component: " + infos[1].samp);
            }

            if (infos[2].samp != 0x11)
            {
                throw new InvalidDataException("Invalid 3rd Component: " + infos[2].samp);
            }

            /* the other components are free to use any quant table but they have to
             * have the same table id */
            if (infos[1].qt != infos[2].qt)
            {
                throw new InvalidDataException("Invalid 3rd Component: different qt");
            }

            return infos;
        }

        public static void gst_rtp_jpeg_pay_read_quant_table(RtpQuantTable[] tables, byte[] data, int size, ref int offset)
        {
            int quant_size, tab_size;
            byte prec;
            byte id;

            if (offset + 2 > size)
            {
                throw new InvalidDataException("DQT needs at least 2 bytes");
            }

            quant_size = gst_rtp_jpeg_pay_header_size(data, offset);
            if (quant_size < 2)
            {
                throw new InvalidDataException("DQT size too small: " + quant_size);
            }

            /* clamp to available data */
            if (offset + quant_size > size)
            {
                quant_size = size - offset;
            }

            offset += 2;
            quant_size -= 2;

            while (quant_size > 0)
            {
                /* not enough to read the id */
                if (offset + 1 > size) break;

                id = (byte)(data[offset] & 0x0f);
                if (id == 15)
                {
                    throw new InvalidDataException("Invalid id 15 received");
                }

                prec = (byte)((data[offset] & 0xf0) >> 4);
                if (prec > 0)
                {
                    tab_size = 128;
                }
                else
                {
                    tab_size = 64;
                }

                /* there is not enough for the table */
                if (quant_size < tab_size + 1)
                {
                    throw new InvalidDataException("Not enough space in DQT for table");
                }

                Console.WriteLine("read quant table{0}, tab_size {1}, prec {2:X2}", id, tab_size, prec);

                tables[id] = new RtpQuantTable { Data = new byte[tab_size] };
                Array.Copy(data, offset + 1, tables[id].Data, 0, tab_size);

                tab_size += 1;
                quant_size -= tab_size;
                offset += tab_size;
            }
        }

        public class RtpJpegHeader
        {
            public byte type_spec;
            public int offset; // 24 bits only
            public byte type;
            public byte q;
            public byte width;
            public byte height;

            public int CopyTo(byte[] data, int offset)
            {
                data[offset++] = this.type_spec;
                data[offset++] = (byte)(this.offset >> 16);
                data[offset++] = (byte)(this.offset >> 8);
                data[offset++] = (byte)this.offset;
                data[offset++] = this.type;
                data[offset++] = this.q;
                data[offset++] = this.width;
                data[offset] = this.height;
                return 8;
            }
        }

        public class RtpQuantTable
        {
            public byte[] Data { get; set; }

            public int Size
            {
                get
                {
                    return this.Data.Length;
                }
            }
        }

        private class CompInfo
        {
            public byte id;

            public byte samp;

            public byte qt;
        }

        private class GstRtpJPEGPay
        {
            public GstRtpJPEGPay()
            {
                this.quality = 255;
                this.quant = 255;
                this.type = 1;
            }

            public byte quality;
            public byte type;

            public int height;

            public int width;

            public byte quant;
        }

        public class RtpQuantHeader
        {
            public byte mbz;
            public byte precision;
            public ushort length;

            public int CopyTo(byte[] data, int offset)
            {
                data[offset++] = this.mbz;
                data[offset++] = this.precision;
                data[offset++] = (byte)(this.length >> 8);
                data[offset] = (byte)this.length;
                return 4;
            }
        }

        private enum RtpJpegMarker : byte
        {
            JPEG_MARKER = 0xFF,
            JPEG_MARKER_SOI = 0xD8,
            JPEG_MARKER_JFIF = 0xE0,
            JPEG_MARKER_CMT = 0xFE,
            JPEG_MARKER_DQT = 0xDB,
            JPEG_MARKER_SOF = 0xC0,
            JPEG_MARKER_DHT = 0xC4,
            JPEG_MARKER_SOS = 0xDA,
            JPEG_MARKER_EOI = 0xD9,
            JPEG_MARKER_DRI = 0xDD,
            JPEG_MARKER_H264 = 0xE4
        }

        private static int jpegCounter;
        private static Font font = new Font(FontFamily.GenericSansSerif, 12f);

        private static byte[] CreateJpeg(out int length)
        {
            //var bmp = new Bitmap(1440, 900);
            var bmp = new Bitmap(640, 480);
            var g = Graphics.FromImage(bmp);

            g.Clear(Color.White);
            g.DrawRectangle(Pens.Blue, 10, 10, bmp.Width - 20, bmp.Height - 20);
            g.DrawString(jpegCounter.ToString(CultureInfo.InvariantCulture), font, Brushes.Red, 20, 20);
            g.DrawString(((int)(jpegCounter / 6.25)).ToString(CultureInfo.InvariantCulture), font, Brushes.Green, 20, 100);

            g.DrawString("Hello world!", font, Brushes.OrangeRed, ((bmp.Width * 100) - (jpegCounter * 3)) % bmp.Width, 150);
            
            // "second hand"
            float x = bmp.Width / 2f;
            float y = bmp.Height / 2f;
            var pen = new Pen(Color.Red, 2);
            g.DrawLine(pen, x, y, x + (float)(Math.Cos(jpegCounter * Math.PI / 188) * 80), y + (float)(Math.Sin(jpegCounter * Math.PI / 188) * 80));

            var memory = new MemoryStream();
            var encoder = Array.Find(ImageCodecInfo.GetImageEncoders(), enc => enc.FormatID.Equals(ImageFormat.Jpeg.Guid));
            var parameters = new EncoderParameters(1);
            parameters.Param[0] = new EncoderParameter(Encoder.Quality, 80L);
            bmp.Save(memory, encoder, parameters);
            memory.Flush();
            var jpeg = memory.GetBuffer();
            length = (int)memory.Length;

            jpegCounter++;
            return jpeg;
        }
    }
}
