using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace MRPApp.View.Process
{
    /// <summary>
    /// ProcessView.xaml에 대한 상호작용 논리
    ///1. 공정계획에서 오늘의 생산계획 일정 불러옴
    ///2. 없으면 에러, 시작버튼 비활성화
    ///3. 있으면 오늘의 날짜 표시, 시작버튼 활성화
    ///4. 시작버튼 클릭 시 새 공정 생성, DB에 입력
    ///   공정코드 : PRC20210618001(PRC+yyyy+MM+dd+NNN)
    ///5. 공정처리 애니메이션 시작
    ///6. 로드타임 후 애니메이션 중지
    ///7. 센서링값 리턴될 때까지 대기
    ///8. 센서링 결과값에 따라 생산품 색상 변경
    ///9. 현재공정의 DB값 업데이트
    ///10. 결과 레이블 값 수정 및 표시
    /// </summary>
    public partial class ProcessView : Page
    {
        //금일 일정
        private Model.Schedules currSchedule;

        public ProcessView()
        {
            InitializeComponent();
        }

        private async void Page_Loaded(object sender, RoutedEventArgs e)
        {
            try
            {
                var today = DateTime.Now.ToString("yyyy-MM-dd");
                currSchedule = Logic.DataAccess.GetSchedules().Where(s => s.PlantCode.Equals(Commons.PLANTCODE))
                    .Where(s => s.SchDate.Equals(DateTime.Parse(today))).FirstOrDefault();
                if (currSchedule == null) 
                {
                    await Commons.ShowMessageAsync("공정", "공정계획이 없습니다. 계획 일정을 먼저 입력하세요.");
                    LblProcessDate.Content = string.Empty;
                    LblSchLoadTime.Content = "None";
                    LblSchAmount.Content = "None";
                    BtnStartProcess.IsEnabled = false;
                    return;
                }
                else
                {
                    //공정계획 표시
                    MessageBox.Show($"{today} 공정 시작합니다.");
                    LblProcessDate.Content = currSchedule.SchDate.ToString("yyyy년 MM월 dd일");
                    LblSchLoadTime.Content = $"{currSchedule.LoadTime} 초";
                    LblSchAmount.Content=$"{currSchedule.SchAmount} 개";
                    BtnStartProcess.IsEnabled = true;
                }
            }
            catch (Exception ex)
            {
                Commons.LOGGER.Error($"예외발생 ProcessView Loaded : {ex}");
                throw ex;
            }
        }

        private void BtnEditMyAccount_Click(object sender, RoutedEventArgs e)
        {

        }

        private void BtnStartProcess_Click(object sender, RoutedEventArgs e)
        {

        }
    }
}
