import { Component, OnInit, ChangeDetectorRef, ViewChildren, QueryList } from '@angular/core';
import { ChartOptions, ChartType, ChartDataSets } from 'chart.js';
import { Label, Color, BaseChartDirective } from 'ng2-charts';
import { DashboardCMService } from './shared/dashboard-cm.service';
import { ToastrService } from 'ngx-toastr';

@Component({
  selector: 'app-dashboard-cm',
  templateUrl: './dashboard-cm.component.html',
  styleUrls: ['./dashboard-cm.component.scss']
})
export class DashboardCMComponent implements OnInit {
  @ViewChildren(BaseChartDirective) charts!: QueryList<BaseChartDirective>;

  tiles: any[] = [];

  // Bar Chart
  barChartLabels: Label[] = [];
  barChartData: ChartDataSets[] = [];
  barChartOptions: ChartOptions = {
    responsive: true,
    maintainAspectRatio: false,
    scales: {
      yAxes: [{
        ticks: { beginAtZero: true, stepSize: 5 },
        gridLines: { color: '#eee' }
      }]
    },
    legend: { display: false }
  };

  // Pie Chart
  pieChartLabels: Label[] = [];
  pieChartData: number[] = [];
  pieChartColors: Color[] = [{ backgroundColor: [] }];
  pieChartOptions: ChartOptions = {
    responsive: true,
    maintainAspectRatio: false,
    legend: {
      position: 'right',
      labels: { fontSize: 13, fontColor: '#333' }
    }
  };

  // Doughnut Chart
  doughnutLabels: Label[] = ['Có mặt', 'Vắng'];
  doughnutData: number[] = [];
  doughnutColors: Color[] = [{ backgroundColor: ['#4dd0e1', '#f44336'] }];
  doughnutOptions: ChartOptions = {
    responsive: true,
    maintainAspectRatio: true,
    cutoutPercentage: 60,
    legend: { position: 'bottom', labels: { fontSize: 13, fontColor: '#333' } }
  };

  attendanceData: any[] = [];
  roomUsageList: any[] = [];
  classAttendanceList: any[] = [];

  private colorList: string[] = [
    '#f44336', '#fdd835', '#4dd0e1', '#8e24aa', '#43a047', '#fb8c00', '#3949ab',
    '#00acc1', '#e53935', '#5e35b1', '#039be5', '#7cb342', '#c0ca33', '#f4511e',
    '#6d4c41', '#00e676', '#1e88e5', '#ffb300', '#8d6e63', '#26c6da', '#ec407a',
    '#ab47bc', '#66bb6a', '#ffa726', '#26a69a'
  ];

  constructor(
    private dashboardService: DashboardCMService,
    private toastr: ToastrService,
    private cdr: ChangeDetectorRef
  ) {}

  ngOnInit(): void {
    this.loadDashboardData();
  }

  loadDashboardData(): void {
    this.dashboardService.getDashboardQuanLyCoSo().subscribe({
      next: (res) => {
        if (!res.isError && res.data) {
          const data = res.data;

          // Tiles
          this.tiles = [
            { label: 'Số học sinh', value: data.soHocSinh, icon: 'ft-user', class: 'tile-student' },
            { label: 'Số giáo viên', value: data.soGiaoVien, icon: 'ft-briefcase', class: 'tile-teacher' },
            { label: 'Số lớp học', value: data.soLopHoc, icon: 'ft-bar-chart', class: 'tile-class' }
          ];

          // Bar Chart
          this.barChartLabels = data.soLopHoc6ThangGanNhat.map((item: any) => `Th ${item.thang}/${item.nam}`);
          this.barChartData = [
            {
              data: data.soLopHoc6ThangGanNhat.map((item: any) => item.soLopHoc),
              label: 'Lớp hoạt động',
              backgroundColor: '#0000ff'
            }
          ];

          // Pie Chart
          const tongSoHocSinh = data.soHocSinh || 1;
          const policies = data.hocSinhTheoChinhSachs.map((item: any) => ({
            label: item.tenChinhSach,
            value: parseFloat(((item.soHocSinh / tongSoHocSinh) * 100).toFixed(1))
          }));
          const totalPoliciesPercent = policies.reduce((sum, p) => sum + p.value, 0);
          const binhThuongPercent = Math.max(0, parseFloat((100 - totalPoliciesPercent).toFixed(1)));

          this.pieChartLabels = policies.map(p => p.label);
          this.pieChartData = policies.map(p => p.value);
          if (binhThuongPercent > 0) {
            this.pieChartLabels.push('Bình thường');
            this.pieChartData.push(binhThuongPercent);
          }

          const pieColors = this.colorList.slice(0, this.pieChartLabels.length);
          this.pieChartColors = [{ backgroundColor: pieColors }];

          // Doughnut Chart
          this.doughnutData = [data.tiLeDiemDanh, 100 - data.tiLeDiemDanh];

          // Room usage 
          this.roomUsageList = data.thoiGianSuDungPhongHocs.map((item: any) => ({
            name: item.tenPhong,
            used: item.thoiGianTrungBinhSuDungPhong
          }));

          // Class attendance
          this.classAttendanceList = data.diemDanhTheoLops.map((item: any) => ({
            name: item.tenLop,
            percentage: item.tiLeDiemDanh,
            totalSessions: item.soBuoiHoc
          }));

          // Student attendance
          this.attendanceData = [];
          data.diemDanhTheoLops.forEach((lop: any) => {
            lop.hocSinhNghiNhieuNhats?.forEach((hocSinh: any) => {
              this.attendanceData.push({
                code: hocSinh.hocSinhCode,
                tenHocSinh: hocSinh.tenHocSinh,
                soBuoiNghi: hocSinh.soBuoiNghi,
                lop: lop.tenLop
              });
            });
          });

          this.cdr.detectChanges();
          this.charts?.forEach(chart => chart.update());
        } else {
          this.toastr.error(res.message || 'Không thể load dữ liệu Dashboard');
        }
      },
      error: (err) => {
        console.error('Lỗi khi lấy Dashboard:', err);
        this.toastr.error('Đã xảy ra lỗi khi lấy dữ liệu Dashboard.');
      }
    });
  }
}
