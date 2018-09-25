namespace JsonMGITester
{
    using System;
    using System.Globalization;
    using System.Net;
    using System.Net.Sockets;
    using System.Text;
    using System.Threading;
    using System.Windows.Forms;

    using JsonMGITester.JsonRpc;

    using Newtonsoft.Json;
    using Newtonsoft.Json.Linq;

    public partial class Form1 : Form
    {
        private readonly TcpClient client = new TcpClient();

        private readonly InfovisionSystemState currentSystemState = new InfovisionSystemState();
        private readonly InfovisionInputState currentInputState = new InfovisionInputState();
        private readonly IbisState currentIbisState = new IbisState();

        private int currentId;

        public Form1()
        {
            this.InitializeComponent();

            this.comboBox1.Items.Add(
                new MgiCall(
                    "InfovisionInputState",
                    "{ \"jsonrpc\": \"2.0\", \"id\": \"1\", \"method\": \"registerObject\", \"params\": [ \"InfovisionInputState\" ] }"));
            this.comboBox1.Items.Add(
                new MgiCall(
                    "IbisState",
                    "{ \"jsonrpc\": \"2.0\", \"id\": \"1\", \"method\": \"registerObject\", \"params\": [ \"IbisState\" ] }"));
            this.comboBox1.Items.Add(
                new MgiCall(
                    "IbisStream",
                    "{ \"jsonrpc\": \"2.0\", \"id\": \"1\", \"method\": \"registerObject\", \"params\": [ \"IbisStream\" ] }"));
            this.comboBox1.Items.Add(
                new MgiCall(
                    "InfoVisionDisplayState",
                    "{ \"jsonrpc\": \"2.0\", \"id\": \"1\", \"method\": \"registerObject\", \"params\": [ \"InfoVisionDisplayState\" ] }"));
        }

        private void button1_Click(object sender, EventArgs e)
        {
            try
            {
                this.client.Connect(IPAddress.Loopback, (int)this.numericUpDown1.Value);
                this.StartReader();

                this.RegisterObject("InfovisionSystemState");
                this.RegisterObject("InfovisionInputState");
                this.RegisterObject("IbisState");
                this.RegisterObject("IbisStream");
            }
            catch (Exception ex)
            {
                this.ShowException(ex);
            }
        }

        private void RegisterObject(string name)
        {
            var id = Interlocked.Increment(ref this.currentId);

            var request = new RpcRequest
                              {
                                  jsonrpc = "2.0",
                                  id = id.ToString(CultureInfo.InvariantCulture),
                                  method = "registerObject",
                                  @params = { name }
                              };


            this.Send(JsonConvert.SerializeObject(request));
        }

        private void StartReader()
        {
            var thread = new Thread(this.ReadLoop);
            thread.IsBackground = true;
            thread.Start();
        }

        private void ShowException(Exception exception)
        {
            if (this.InvokeRequired)
            {
                this.Invoke(new MethodInvoker(() => this.ShowException(exception)));
                return;
            }

            MessageBox.Show(
                this, exception.ToString(), exception.GetType().Name, MessageBoxButtons.OK, MessageBoxIcon.Error);
        }

        private void ShowInfo(string info)
        {
            if (this.InvokeRequired)
            {
                this.Invoke(new MethodInvoker(() => this.ShowInfo(info)));
                return;
            }

            if (this.tabControl1.SelectedTab != this.tabPage2)
            {
                // only show info messageboxes when in manual mode
                return;
            }

            MessageBox.Show(this, info, "Information", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void ReadLoop()
        {
            try
            {
                var buffer = new byte[10240];
                var stream = this.client.GetStream();
                var serializer = new JsonSerializer();
                var reader = new JsonStreamReader(stream);
                while (true)
                {
                    /*var read = stream.Read(buffer, 0, buffer.Length);
                    if (read == 0)
                    {
                        continue;
                    }

                    var text = Encoding.ASCII.GetString(buffer, 0, read);
                    this.Invoke(new MethodInvoker(() => this.textBox2.AppendText(text)));*/

                    //var reader = new StringReader(text);
                    //var jsonReader = new JsonTextReader(reader);
                    //jsonReader.CloseInput = false;
                    var obj = reader.ReadObject();
                    this.Invoke(new MethodInvoker(() => this.textBox2.AppendText(obj.ToString())));
                    this.HandleObject(obj as JObject);
                }
            }
            catch (Exception ex)
            {
                this.ShowException(ex);
            }
        }

        private void HandleObject(JObject obj)
        {
            if (obj == null)
            {
                return;
            }

            JToken versionToken;
            obj.TryGetValue("jsonrpc", out versionToken);
            var version = versionToken as JValue;
            if (version == null || version.ToString() != "2.0")
            {
                this.ShowException(new NotSupportedException("Unsupported version: " + version));
                return;
            }

            JToken methodToken;
            obj.TryGetValue("method", out methodToken);
            var method = methodToken as JValue;

            if (method != null)
            {
                JToken paramsToken;
                obj.TryGetValue("params", out paramsToken);
                this.InvokeMethod(method.ToString(), paramsToken as JContainer);
                return;
            }

            JToken idToken;
            obj.TryGetValue("id", out idToken);
            var id = idToken as JValue;
            if (id == null)
            {
                this.ShowException(new NotSupportedException("No id available"));
                return;
            }

            // TODO: handle result / error
            this.ShowInfo(obj.ToString());
        }

        private void InvokeMethod(string method, JContainer parameters)
        {
            if (method != "notifyObject")
            {
                this.ShowException(new NotSupportedException("Unsupported method: " + method));
                return;
            }

            var paramList = parameters as JObject;
            if (paramList == null)
            {
                this.ShowException(new NotSupportedException("Missing params for notifyObject()"));
                return;
            }

            JToken objectNameToken;
            paramList.TryGetValue("objectName", out objectNameToken);
            var objectName = objectNameToken as JValue;
            if (objectName == null)
            {
                this.ShowException(new NotSupportedException("Missing objectName for notifyObject()"));
                return;
            }

            paramList.Remove("objectName");

            this.ShowInfo("Notified Object '" + objectName + "':\r\n" + paramList);

            switch (objectName.ToString())
            {
                case "InfovisionSystemState":
                    this.NotifyInfovisionSystemState(this.CreateObject<InfovisionSystemState>(paramList));
                    break;
                case "InfovisionInputState":
                    this.NotifyInfovisionInputState(this.CreateObject<InfovisionInputState>(paramList));
                    break;
                case "IbisState":
                    this.NotifyIbisState(this.CreateObject<IbisState>(paramList));
                    break;
                case "IbisStream":
                    this.NotifyIbisStream(this.CreateObject<IbisStream>(paramList));
                    break;
            }
        }

        private void NotifyInfovisionSystemState(InfovisionSystemState state)
        {
            if (this.InvokeRequired)
            {
                this.Invoke(new MethodInvoker(() => this.NotifyInfovisionSystemState(state)));
                return;
            }

            this.currentSystemState.Serial = state.Serial ?? this.currentSystemState.Serial;
            this.currentSystemState.HWRef = state.HWRef ?? this.currentSystemState.HWRef;
            this.currentSystemState.At91Version = state.At91Version ?? this.currentSystemState.At91Version;
            this.currentSystemState.At91Rev = state.At91Rev ?? this.currentSystemState.At91Rev;
            this.currentSystemState.FanSpeedRPM = state.FanSpeedRPM ?? this.currentSystemState.FanSpeedRPM;
            this.currentSystemState.Temperature = state.Temperature ?? this.currentSystemState.Temperature;

            this.propertyGridSystemParameters.SelectedObject = this.currentSystemState;
        }

        private void NotifyInfovisionInputState(InfovisionInputState state)
        {
            if (this.InvokeRequired)
            {
                this.Invoke(new MethodInvoker(() => this.NotifyInfovisionInputState(state)));
                return;
            }

            this.currentInputState.Stop0 = state.Stop0 ?? this.currentInputState.Stop0;
            this.currentInputState.Stop1 = state.Stop1 ?? this.currentInputState.Stop1;
            this.currentInputState.Ignition = state.Ignition ?? this.currentInputState.Ignition;
            this.currentInputState.Address = state.Address ?? this.currentInputState.Address;

            this.propertyGridInputState.SelectedObject = this.currentInputState;
        }

        private void NotifyIbisState(IbisState state)
        {
            if (this.InvokeRequired)
            {
                this.Invoke(new MethodInvoker(() => this.NotifyIbisState(state)));
                return;
            }

            this.currentIbisState.LineNo = state.LineNo ?? this.currentIbisState.LineNo;
            this.currentIbisState.CourseNo = state.CourseNo ?? this.currentIbisState.CourseNo;
            this.currentIbisState.RouteNo = state.RouteNo ?? this.currentIbisState.RouteNo;
            this.currentIbisState.StopNo = state.StopNo ?? this.currentIbisState.StopNo;
            this.currentIbisState.AnnounceNo = state.AnnounceNo ?? this.currentIbisState.AnnounceNo;

            this.propertyGridIbisState.SelectedObject = this.currentIbisState;
        }

        private void NotifyIbisStream(IbisStream stream)
        {
            if (this.InvokeRequired)
            {
                this.Invoke(new MethodInvoker(() => this.NotifyIbisStream(stream)));
                return;
            }

            var timestamp = DateTime.Now.ToString("HH:mm:ss.fff");
            this.listBox1.BeginUpdate();
            foreach (var telegram in stream.Data)
            {
                this.listBox1.Items.Insert(0, timestamp + "  " + telegram);
            }

            this.listBox1.EndUpdate();
        }

        private T CreateObject<T>(JObject paramList)
        {
            var reader = paramList.CreateReader();
            var serializer = new JsonSerializer();
            return serializer.Deserialize<T>(reader);
        }

        private void button2_Click(object sender, EventArgs e)
        {
            this.Send(this.textBox1.Text);
        }

        private void Send(string text)
        {
            var data = Encoding.ASCII.GetBytes(text);
            try
            {
                this.client.GetStream().Write(data, 0, data.Length);
            }
            catch (Exception ex)
            {
                this.ShowException(ex);
            }
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            var call = this.comboBox1.SelectedItem as MgiCall;
            if (call == null)
            {
                return;
            }

            this.textBox1.Text = call.Command;
            this.comboBox1.SelectedIndex = -1;
        }

        private class MgiCall
        {
            public MgiCall(string name, string command)
            {
                this.Name = name;
                this.Command = command;
            }

            public string Name { get; private set; }

            public string Command { get; private set; }

            public override string ToString()
            {
                return this.Name;
            }
        }
    }
}
