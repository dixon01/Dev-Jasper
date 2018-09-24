// -----------------------------------------------------------------------
// <copyright file="PurchaseOrderXmlInMemoryStream.cs" company="">
// TODO: Update copyright text.
// </copyright>
// -----------------------------------------------------------------------

namespace Gorba.Common.Configuration.Core.Tests
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Text;

    /// <summary>
    /// TODO: Update summary.
    /// </summary>
    public class PurchaseOrderXmlInMemoryStream
    {
        public static MemoryStream GetInMemorySerializedPurchaseOrder()
        {
            var stream = new MemoryStream();
            byte[] buffer = GetPurchaseOrderXmlFileBuffer();
            stream.Write(buffer, 0, buffer.Length);

            return stream;
        }

        private static byte[] GetPurchaseOrderXmlFileBuffer()
        {
            var str = new StringBuilder();
            str.Append("<?xml version=\"1.0\"?>");
            str.Append(Environment.NewLine);
            str.Append("<PurchaseOrder xmlns:xsi=\"http://www.w3.org/2001/XMLSchema-instance\" xmlns:xsd=\"http://www.w3.org/2001/XMLSchema\" xmlns=\"Gorba.test.PurchaseOrder\">");
            str.Append(Environment.NewLine);
            str.Append("  <ShipTo Name=\"Teresa Atkinson\">");
            str.Append(Environment.NewLine);
            str.Append("    <Line1>1 Main St.</Line1>");
            str.Append(Environment.NewLine);
            str.Append("    <City>AnyTown</City>");
            str.Append(Environment.NewLine);
            str.Append("    <State>WA</State>");
            str.Append(Environment.NewLine);
            str.Append("    <Zip>41120</Zip>");
            str.Append(Environment.NewLine);
            str.Append("  </ShipTo>");
            str.Append(Environment.NewLine);
            str.Append(string.Format("  <OrderDate>{0}</OrderDate>", new DateTime(2012, 02, 10).ToShortDateString()));
            str.Append(Environment.NewLine);
            str.Append("  <Items>");
            str.Append(Environment.NewLine);
            str.Append("    <OrderedItem>");
            str.Append(Environment.NewLine);
            str.Append("      <ItemName>Widget S</ItemName>");
            str.Append(Environment.NewLine);
            str.Append("      <Description>Small widget</Description>");
            str.Append(Environment.NewLine);
            str.Append("      <UnitPrice>5.23</UnitPrice>");
            str.Append(Environment.NewLine);
            str.Append("      <Quantity>3</Quantity>");
            str.Append(Environment.NewLine);
            str.Append("      <LineTotal>15.69</LineTotal>");
            str.Append(Environment.NewLine);
            str.Append("    </OrderedItem>");
            str.Append(Environment.NewLine);
            str.Append("    <OrderedItem>");
            str.Append(Environment.NewLine);
            str.Append("      <ItemName>I 2</ItemName>");
            str.Append(Environment.NewLine);
            str.Append("      <Description>Second item</Description>");
            str.Append(Environment.NewLine);
            str.Append("      <UnitPrice>4.89</UnitPrice>");
            str.Append(Environment.NewLine);
            str.Append("      <Quantity>5</Quantity>");
            str.Append(Environment.NewLine);
            str.Append("      <LineTotal>24.45</LineTotal>");
            str.Append(Environment.NewLine);
            str.Append("    </OrderedItem>");
            str.Append(Environment.NewLine);
            str.Append("  </Items>");
            str.Append(Environment.NewLine);
            str.Append("  <SubTotal>40.14</SubTotal>");
            str.Append(Environment.NewLine);
            str.Append("  <ShipCost>12.51</ShipCost>");
            str.Append(Environment.NewLine);
            str.Append("  <TotalCost>52.65</TotalCost>");
            str.Append(Environment.NewLine);
            str.Append("</PurchaseOrder>");

            return Encoding.ASCII.GetBytes(str.ToString());
        }
    }
}
