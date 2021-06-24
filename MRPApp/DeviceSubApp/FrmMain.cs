using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using uPLibrary.Networking.M2Mqtt;
using uPLibrary.Networking.M2Mqtt.Messages;

namespace DeviceSubApp
{
    public partial class FrmMain : Form
    {
        MqttClient client;
        string connectionString; //DB 연결 문자열 or MQTT Broker address
        ulong lineCount; //richtextbox에 글 적을 때 라인넘버 생기게 하는 것
        delegate void UpdateTextCallback(string message); //thread상에서 윈폼 richtextbox에 text 출력 시 필요

        Stopwatch sw = new Stopwatch();

        public FrmMain()
        {
            InitializeComponent();
            IntiailizeAllData(); //초기화
        }

        private void IntiailizeAllData() //초기화
        {
            //DB 연결
            connectionString = "Data Source=" + TxtConString.Text +";Initial Catalog=MRP;" +
                "Persist Security Info=True;User ID=sa;password=mssql_p@ssw0rd!";
            lineCount = 0;

            BtnConnect.Enabled = true; //접속 전이므로 버튼 활성화
            BtnDisconnect.Enabled = false; //== disabled, 접속 전이므로 접속해제 비활성화
            IPAddress brokerAddress;

            //오류발생 가능성있으므로 예외처리
            try
            {
                brokerAddress = IPAddress.Parse(TxtConString.Text); //TxtConString.Text를 IPAddress로 파싱(형변환)
                client = new MqttClient(brokerAddress);
                client.MqttMsgPublishReceived += Client_MqttMsgPublishReceived;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString()); //오류나는 부분을 메세지박스로 출력
            }

            Timer.Enabled = true;
            Timer.Interval = 1000; //1초마다(1000ms == 1s)
            Timer.Tick += Timer_Tick; //1초마다 tick할 때 이벤트 수행
            Timer.Start();
        }

        private void Timer_Tick(object sender, EventArgs e)
        {
            LblResult.Text = sw.Elapsed.Seconds.ToString();
            if (sw.Elapsed.Seconds >= 3)
            {
                sw.Stop();
                sw.Reset();
                //TODO 실제 처리 프로세스 실행
                UpdateText("처리!");
                PrcCorrectDataToDB();
                //ClearData();
            }
        }

        //여러 데이터 중 최종 데이터만 DB에 입력
        private void PrcCorrectDataToDB()
        {
            if (iotData.Count > 0)
            {
                var correctData = iotData[iotData.Count - 1]; //iotData.Count - 1 : 마지막 값
                //DB에 입력
                //UpdateText("DB처리");
                using (var conn = new SqlConnection(connectionString)) 
                {
                    var prcResult = correctData["PRC_MSG"] == "OK" ? 1 : 0;
                    string strUpQry = $"UPDATE Process_DEV "+
                                      $"   SET PrcEndTime = '{DateTime.Now.ToString("HH:mm:ss")}' " +
                                      $"     , PrcResult = '{prcResult}' " +
                                      $"     , ModDate = '{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")}' " +
                                      $"     , ModID = '{"SYS"}' " +
                                      $" WHERE PrcIdx = " +
                                      $" (SELECT TOP 1 PrcIdx FROM Process_DEV ORDER BY PrcIdx DESC)";
                    try
                    {
                        conn.Open();
                        SqlCommand cmd = new SqlCommand(strUpQry, conn);
                        if (cmd.ExecuteNonQuery() == 1)
                            UpdateText("[DB] 센싱값 Update 성공");
                        else
                            UpdateText("[DB] 센싱값 Update 실패");
                    }
                    catch (Exception ex)
                    {
                        UpdateText($">>>>> DB ERROR! : {ex.Message}");
                    }
                }
            }
            iotData.Clear(); //데이터 모두 삭제
        }

        private void Client_MqttMsgPublishReceived(object sender, MqttMsgPublishEventArgs e) //e로 메세지 받아옴
        {
            try
            {
                var message = Encoding.UTF8.GetString(e.Message);
                UpdateText($">>>>> 받은 메세지 : {message}");
                // message(json) -> C#
                var currentData = JsonConvert.DeserializeObject<Dictionary<string, string>>(message); //역직렬화, serializeObject : 직렬화
                PrcInputDataToList(currentData);



                //TODO : 메세지 받은 이후에 처리 -> stopwatch로 설정
                sw.Stop();
                sw.Reset();
                sw.Start();
            }
            catch (Exception ex)
            {
                UpdateText($">>>>> ERROR! : {ex.Message}");
            }
        }

        List<Dictionary<string, string>> iotData = new List<Dictionary<string, string>>();

        //라즈베리에서 들어온 메세지를 전역 리스트에 입력하는 메소드
        private void PrcInputDataToList(Dictionary<string, string> currentData)
        {
            if(currentData["PRC_MSG"] == "OK" || currentData["PRC_MSG"] == "FAIL")
                iotData.Add(currentData);
        }

        private void BtnConnect_Click(object sender, EventArgs e)
        {
            client.Connect(TxtClntID.Text); //SUBSCR01
            UpdateText(">>>>> Client Connected");
            client.Subscribe(new string[] { TxtSubTopic.Text }, new byte[] { MqttMsgBase.QOS_LEVEL_AT_MOST_ONCE}); //MqttMsgBase.QOS_LEVEL_AT_MOST_ONCE = 0 : 최대 1번만 데이터 전송
            UpdateText(">>>>> Subscribing to : " + TxtSubTopic.Text);
           
            BtnConnect.Enabled = false; //접속했으므로 버튼 비활성화
            BtnDisconnect.Enabled = true; 
        }

        private void BtnDisconnect_Click(object sender, EventArgs e)
        {
            client.Disconnect(); //접속해제
            UpdateText(">>>>> Client Disconnected!");

            BtnConnect.Enabled = true;
            BtnDisconnect.Enabled = false;
        }

        private void UpdateText(string message) //delegate와 인수 개수가 일치해야 함
        {
            if (RtbSub.InvokeRequired)
            {
                UpdateTextCallback callback = new UpdateTextCallback(UpdateText);
                this.Invoke(callback, new object[] { message });
            }
            else
            {
                lineCount++;
                RtbSub.AppendText($"{lineCount} : {message}\n");
                RtbSub.ScrollToCaret();
            }
        }
    }
}
