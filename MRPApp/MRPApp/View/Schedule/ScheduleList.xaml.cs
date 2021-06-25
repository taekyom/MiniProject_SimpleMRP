using MRPApp.View.Store;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace MRPApp.View.Schedule
{
    /// <summary>
    /// MyAccount.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class ScheduleList : Page
    {
        public ScheduleList()
        {
            InitializeComponent();
        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            try
            {
                LoadControlData(); //콤보박스 데이터 로딩
                LoadGridData(); //테이블 데이터 그리드 표시
                InitErrorMessages();

            }
            catch (Exception ex)
            {
                Commons.LOGGER.Error($"예외발생 StoreList Loaded : {ex}");
                throw ex;
            }
        }

        private void LoadControlData()
        {
            var plantCodes = Logic.DataAccess.GetSettings().Where(c => c.BasicCode.Contains("PC01")).ToList();
            CboPlantCode.ItemsSource = plantCodes;
            CboGrdPlantCode.ItemsSource = plantCodes;

            var facilityIDs = Logic.DataAccess.GetSettings().Where(c => c.BasicCode.Contains("FAC1")).ToList();
            CboSchFacilityID.ItemsSource = facilityIDs;
        }

        private void InitErrorMessages()
        {
            LblPlantCode.Visibility = LblSchDate.Visibility = LblSchEndTime.Visibility = LblSchLoadTime.Visibility = 
                LblSchStartTime.Visibility  = LblSchAmount.Visibility = LblSchFacilityID.Visibility = Visibility.Hidden;
        }

        private void LoadGridData()
        {
            List<Model.Schedules> settings = Logic.DataAccess.GetSchedules();
            this.DataContext = settings;
        }

        private void BtnEditUser_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                //NavigationService.Navigate(new EditUser());
            }
            catch (Exception ex)
            {
                Commons.LOGGER.Error($"예외발생 BtnEditUser_Click : {ex}");
                throw ex;
            }
        }

        private void BtnAddStore_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                NavigationService.Navigate(new AddStore());
            }
            catch (Exception ex)
            {
                Commons.LOGGER.Error($"예외발생 BtnAddStore_Click : {ex}");
                throw ex;
            }
        }

        private void BtnEditStore_Click(object sender, RoutedEventArgs e)
        {
            if (GrdData.SelectedItem == null)
            {
                Commons.ShowMessageAsync("창고수정", "수정할 창고를 선택하세요");
                return;
            }

            try
            {
                
            }
            catch (Exception ex)
            {
                Commons.LOGGER.Error($"예외발생 BtnEditStore_Click : {ex}");
                throw ex;
            }
        }

        private void BtnExportExcel_Click(object sender, RoutedEventArgs e)
        {
           
        }

        private void BtnNew_Click(object sender, RoutedEventArgs e)
        {
            ClearInput();
        }

        private async void BtnInsert_Click(object sender, RoutedEventArgs e)
        {
            if (IsVaildInput() != true) return;

            var item = new Model.Schedules();
            item.PlantCode = CboPlantCode.SelectedValue.ToString();
            item.SchDate = DateTime.Parse(DtpSchDate.Text);
            item.LoadTime = int.Parse(TxtSchLoadTime.Text);
            item.SchStartTime = TmpSchStartTime.SelectedDateTime.Value.TimeOfDay;
            item.SchEndTime = TmpSchEndTime.SelectedDateTime.Value.TimeOfDay;
            item.SchFacilityID = CboSchFacilityID.SelectedValue.ToString();
            item.SchAmount = (int)NudSchAmount.Value;

            item.RegDate = DateTime.Now;
            item.RegID = "MRP";

            try
            {
                var result = Logic.DataAccess.SetSchedule(item);
                if (result == 0)
                {
                    Commons.LOGGER.Error("데이터 수정 시 오류 발생");
                    await Commons.ShowMessageAsync("오류", "데이터 수정 실패!");
                }
                else
                {
                    Commons.LOGGER.Info($"데이터 수정 성공 : {item.SchIdx}"); //로그
                    ClearInput();
                    LoadGridData();
                }
            }
            catch (Exception ex)
            {
                Commons.LOGGER.Error($"예외발생 {ex}");
            }
        }

        //입력데이터 검증 메소드
        public bool IsVaildInput()
        {
            var isVaild = true;
            InitErrorMessages();

            if (CboPlantCode.SelectedValue == null)
            {
                LblPlantCode.Visibility = Visibility.Visible;
                LblPlantCode.Text = "공장을 선택하세요";
                isVaild = false;
            }

            if (string.IsNullOrEmpty(DtpSchDate.Text))
            {
                LblSchDate.Visibility = Visibility.Visible;
                LblSchDate.Text = "공정일을 입력하세요";
                isVaild = false;
            }

            if (CboPlantCode.SelectedValue != null && string.IsNullOrEmpty(DtpSchDate.Text))
            {
                //공장별로 공정일이 DB값에 있으면 입력되면 안됨(중복입력 불허)
                //PC01001(수원) 2021-06-24
                var result = Logic.DataAccess.GetSchedules().Where(s => s.PlantCode.Equals(CboPlantCode.SelectedValue.ToString()))
                    .Where(d => d.SchDate.Equals(DateTime.Parse(DtpSchDate.Text))).Count();
                if (result > 0)
                {
                    LblSchDate.Visibility = Visibility.Visible;
                    LblSchDate.Text = "해당 공장 공정일에 계획이 이미 존재합니다.";
                }
            }
            

            if (string.IsNullOrEmpty(TxtSchLoadTime.Text))
            {
                LblSchLoadTime.Visibility = Visibility.Visible;
                LblSchLoadTime.Text = "로드 타임을 입력하세요";
                isVaild = false;
            }

            if (CboSchFacilityID.SelectedValue == null) 
            {
                LblSchFacilityID.Visibility = Visibility.Visible;
                LblSchFacilityID.Text = "공정 설비를 선택하세요";
                isVaild = false;
            }

            if (NudSchAmount.Value <= 0) 
            {
                LblSchAmount.Visibility = Visibility.Visible;
                LblSchAmount.Text = "계획 수량은 0개 이상입니다.";
            }


            //if (string.IsNullOrEmpty(TxtBasicCode.Text))
            //{
            //    LblBasicCode.Visibility = Visibility.Visible;
            //    LblBasicCode.Text = "코드를 입력하세요";
            //    isVaild = false;
            //}
            //else if (Logic.DataAccess.GetSettings().Where(s => s.BasicCode.Equals(TxtBasicCode.Text)).Count() > 0) 
            //{
            //    LblBasicCode.Visibility = Visibility.Visible;
            //    LblBasicCode.Text = "중복코드가 존재합니다";
            //    isVaild = false;
            //}

            //if (string.IsNullOrEmpty(TxtCodeName.Text))
            //{
            //    LblCodeName.Visibility = Visibility.Visible;
            //    LblCodeName.Text = "코드명을 입력하세요";
            //    isVaild = false;
            //}
            return isVaild;
        }

        private async void BtnUpdate_Click(object sender, RoutedEventArgs e)
        {
            //setting.CodeName = TxtCodeName.Text;
            //setting.CodeDesc = TxtCodeDesc.Text;
            //setting.ModDate = DateTime.Now;
            //setting.ModeID = "MRP";

            var item = GrdData.SelectedItems as Model.Schedules;
            item.PlantCode = CboPlantCode.SelectedValue.ToString();
            item.SchDate = DateTime.Parse(DtpSchDate.Text);
            item.LoadTime = int.Parse(TxtSchLoadTime.Text);
            item.SchStartTime = TmpSchStartTime.SelectedDateTime.Value.TimeOfDay;
            item.SchEndTime = TmpSchEndTime.SelectedDateTime.Value.TimeOfDay;
            item.SchFacilityID = CboSchFacilityID.SelectedValue.ToString();
            item.SchAmount = (int)NudSchAmount.Value;

            item.ModDate = DateTime.Now;
            item.ModID = "MRP";

            try
            {
                var result = Logic.DataAccess.SetSchedule(item);
                if (result == 0)
                {
                    Commons.LOGGER.Error("데이터 수정시 오류 발생");
                    await Commons.ShowMessageAsync("오류", "데이터 수정 실패!!");
                }
                else
                {
                    Commons.LOGGER.Info($"데이터 수정 성공 : {item.SchIdx}");
                    ClearInput();
                    LoadGridData();
                }
            }
            catch (Exception ex)
            {
                Commons.LOGGER.Error($"예외발생{ex}");
            }

        }

        private void ClearInput()
        {
            TxtSchIdx.Text = "";
            CboPlantCode.SelectedItem = null;
            DtpSchDate.Text = "";
            TxtSchLoadTime.Text = "";
            TmpSchStartTime.SelectedDateTime = null;
            TmpSchEndTime.SelectedDateTime = null;
            CboSchFacilityID.SelectedItem = null;
            NudSchAmount.Value = 0;
            CboPlantCode.Focus();
        }

        private void BtnSearch_Click(object sender, RoutedEventArgs e)
        {
            var search = DtpSearchDate.Text;
            var settings = Logic.DataAccess.GetSchedules().Where(s => s.SchDate.Equals(DateTime.Parse(search))).ToList();
            this.DataContext = settings;
        }

        private void GrdData_SelectedCellsChanged(object sender, SelectedCellsChangedEventArgs e)
        {
            try
            {
                var item = GrdData.SelectedItem as Model.Schedules;
                TxtSchIdx.Text = item.SchIdx.ToString();
                CboPlantCode.SelectedValue = item.PlantCode;
                DtpSchDate.Text = item.SchDate.ToString();
                TxtSchLoadTime.Text = item.LoadTime.ToString();
                TmpSchStartTime.SelectedDateTime = new DateTime(item.SchStartTime.Value.Ticks);
                TmpSchEndTime.SelectedDateTime = new DateTime(item.SchEndTime.Value.Ticks);

                CboSchFacilityID.SelectedValue = item.SchFacilityID;
                NudSchAmount.Value = item.SchAmount;
            }
            catch (Exception ex)
            {
                Commons.LOGGER.Error($"예외 발생 {ex}");
                ClearInput();
            }
        }
    }
}
