import {
  Component,
  OnInit,
  ChangeDetectorRef,
  ViewChildren,
  QueryList,
} from "@angular/core";
import { BaseChartDirective } from "ng2-charts";
import { ChartOptions, ChartType, ChartDataSets } from "chart.js";
import { Label, Color } from "ng2-charts";
import { DashboardAdminService } from "./shared/dasboard-admin.service";
import { ToastrService } from "ngx-toastr";

@Component({
  selector: "app-dashboard-admin",
  templateUrl: "./dashboard-admin.component.html",
  styleUrls: ["./dashboard-admin.component.scss"],
})
export class DashboardAdminComponent implements OnInit {
  @ViewChildren(BaseChartDirective) charts!: QueryList<BaseChartDirective>;

  tiles: any[] = [];
  tiles2: any[] = [];

  barChartLabels: Label[] = [];
  barChartData: ChartDataSets[] = [];

  pieChartLabels: Label[] = [];
  pieChartData: number[] = [];
  barChartCanvasMinWidth: number = 600;
  pieChartColors: Color[] = [
    { backgroundColor: ["#f44336", "#fdd835", "#4dd0e1"] },
  ];

  doughnutLabels: Label[] = ["Có mặt", "Vắng"];
  doughnutData: number[] = [];
  doughnutColors: Color[] = [{ backgroundColor: ["#4dd0e1", "#f44336"] }];

  horizontalLabels: Label[] = [];
  horizontalData: ChartDataSets[] = [];

  // Chart Options
  barChartOptions: ChartOptions = {
    responsive: true,
    maintainAspectRatio: false,
    scales: {
      xAxes: [
        {
          gridLines: { display: false },
          ticks: { fontSize: 12, maxRotation: 0, minRotation: 0 },
        },
      ],
      yAxes: [
        {
          ticks: { beginAtZero: true, stepSize: 5, fontSize: 12 },
          gridLines: { color: "#eee" },
        },
      ],
    },
    legend: {
      position: "top",
      labels: { fontSize: 13, fontColor: "#333", usePointStyle: true },
    },
    tooltips: { enabled: true },
  };

  pieChartOptions: ChartOptions = {
    responsive: true,
    maintainAspectRatio: false,
    legend: {
      position: "bottom",
      labels: {
        fontSize: 13,
        fontColor: "#333",
        boxWidth: 12,
        padding: 10,
      },
    },
    tooltips: {
      callbacks: {
        label: function (tooltipItem, data) {
          const label = data.labels[tooltipItem.index] || "";
          const value =
            data.datasets[tooltipItem.datasetIndex].data[tooltipItem.index];
          return `${label}: ${value}%`;
        },
      },
    },
  };

  doughnutOptions: ChartOptions = {
    responsive: true,
    maintainAspectRatio: true,
    cutoutPercentage: 60,
    legend: { position: "bottom", labels: { fontSize: 13, fontColor: "#333" } },
  };

  horizontalOptions: ChartOptions = {
    responsive: true,
    maintainAspectRatio: false,
    scales: {
      xAxes: [
        {
          ticks: { beginAtZero: true, stepSize: 3 },
          gridLines: { color: "#eee" },
        },
      ],
      yAxes: [{ gridLines: { color: "#eee" }, ticks: { fontSize: 12 } }],
    },
    legend: { display: false },
  };

  constructor(
    private dashboardService: DashboardAdminService,
    private toastr: ToastrService,
    private cdr: ChangeDetectorRef
  ) {}

  ngOnInit(): void {
    this.loadDashboardData();
  }

  private getRandomColor(): string {
    const letters = "0123456789ABCDEF";
    let color = "#";
    for (let i = 0; i < 6; i++) {
      color += letters[Math.floor(Math.random() * 16)];
    }
    return color;
  }
  private colorList: string[] = [
    "#f44336",
    "#fdd835",
    "#4dd0e1",
    "#8e24aa",
    "#43a047",
    "#fb8c00",
    "#3949ab",
    "#00acc1",
    "#e53935",
    "#5e35b1",
    "#039be5",
    "#7cb342",
    "#c0ca33",
    "#f4511e",
    "#6d4c41",
    "#00e676",
    "#1e88e5",
    "#ffb300",
    "#8d6e63",
    "#26c6da",
    "#ec407a",
    "#ab47bc",
    "#66bb6a",
    "#ffa726",
    "#26a69a",
  ];

  loadDashboardData(): void {
    this.dashboardService.getDashboardAdmin().subscribe({
      next: (res) => {
        if (!res.isError && res.data) {
          const data = res.data;

          // Update Tiles
          this.tiles = [
            {
              label: "Số học sinh",
              value: data.soHocSinh,
              icon: "ft-user",
              class: "tile-student",
            },
            {
              label: "Số giáo viên",
              value: data.soGiaoVien,
              icon: "ft-briefcase",
              class: "tile-teacher",
            },
            {
              label: "Số nhân viên",
              value: data.soNhanVien,
              icon: "ft-bar-chart",
              class: "tile-employee",
            },
          ];
          this.tiles2 = [
            { label: "Số lớp học", value: data.soLopHoc, icon: "ft-user" },
            {
              label: "Số lớp đang diễn ra",
              value: data.soLopHocDangDiemRa,
              icon: "ft-user",
            },
          ];

          // Update Bar Chart
          this.barChartLabels = data.hocSinhGiaoVienLopHocTheoCoSos.map(
            (item: any) => item.tenCoSo
          );
          this.barChartCanvasMinWidth = Math.max(
            this.barChartLabels.length * 120,
            600
          );
          this.barChartData = [
            {
              data: data.hocSinhGiaoVienLopHocTheoCoSos.map(
                (item: any) => item.soHocSinh
              ),
              label: "Học sinh",
              backgroundColor: "#0000ff",
              hoverBackgroundColor: "#0000ff",
              barPercentage: 0.8,
              categoryPercentage: 0.5,
            },
            {
              data: data.hocSinhGiaoVienLopHocTheoCoSos.map(
                (item: any) => item.soGiaoVien
              ),
              label: "Giáo viên",
              backgroundColor: "#00cccc",
              hoverBackgroundColor: "#00cccc",
              barPercentage: 0.8,
              categoryPercentage: 0.5,
            },
          ];

          // Update Pie Chart
          this.pieChartLabels = data.hocSinhTheoChinhSachs.map(
            (item: any) => item.tenChinhSach
          );
          const tongSoHocSinh = data.soHocSinh || 1;
          this.pieChartData = data.hocSinhTheoChinhSachs.map((item: any) =>
            parseFloat(((item.soHocSinh / tongSoHocSinh) * 100).toFixed(1))
          );
          const totalOtherPoliciesPercent = this.pieChartData.reduce(
            (sum, val) => sum + val,
            0
          );

          const normalPolicyPercent = parseFloat(
            (100 - totalOtherPoliciesPercent).toFixed(1)
          );

          if (normalPolicyPercent > 0) {
            this.pieChartLabels.push("Không có chính sách");
            this.pieChartData.push(normalPolicyPercent);

            const extraColor = "#808080";
            const currentColors = this.pieChartColors[0]
              .backgroundColor as string[];
            currentColors.push(extraColor);
          }

          const pieColors = this.colorList.slice(0, this.pieChartLabels.length);
          this.pieChartColors = [{ backgroundColor: pieColors }];

          // Update Doughnut Chart
          const tiLeDiemDanh = data.tiLeDiemDanh ?? 0;
          this.doughnutData = [
            100 - tiLeDiemDanh,
            tiLeDiemDanh, 
          ];

          // Update Horizontal Bar
          this.horizontalLabels = data.hocSinhGiaoVienLopHocTheoCoSos.map(
            (item: any) => item.tenCoSo
          );
          this.horizontalData = [
            {
              data: data.hocSinhGiaoVienLopHocTheoCoSos.map(
                (item: any) => item.soLopHoc
              ),
              label: "Lớp",
              backgroundColor: "#ff9800",
            },
          ];

          // Cập nhật ngay lập tức sau khi gán data
          this.cdr.detectChanges();
          requestAnimationFrame(() => {
            this.charts?.forEach((chart) => {
              chart.chart?.resize(); // Resize lại
              chart.update(); // Update dữ liệu
            });
          });
        } else {
          this.toastr.error(res.message || "Không thể load dữ liệu Dashboard");
        }
      },
      error: (err) => {
        console.error("❌ Lỗi khi lấy Dashboard:", err);
        this.toastr.error("Đã xảy ra lỗi khi lấy dữ liệu Dashboard.");
      },
    });
  }
  private updateCharts(): void {
    if (this.charts && this.charts.length > 0) {
      this.charts.forEach((chart) => {
        chart.update();
      });
    }
  }
}
