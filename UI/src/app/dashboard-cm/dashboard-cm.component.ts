import { Component, OnInit } from '@angular/core';
import { ChartOptions, ChartType, ChartDataSets } from 'chart.js';
import { Label, Color } from 'ng2-charts';

@Component({
  selector: 'app-dashboard-cm',
  templateUrl: './dashboard-cm.component.html',
  styleUrls: ['./dashboard-cm.component.scss']
})
export class DashboardCMComponent implements OnInit {
  canvasHeight: number = 0;
  // Top tiles
  tiles = [
    { label: 'Số học sinh', value: 200, icon: 'ft-user', class: 'tile-student' },
    { label: 'Số giáo viên', value: 30, icon: 'ft-briefcase', class: 'tile-teacher' },
    { label: 'Số lớp học', value: 15, icon: 'ft-bar-chart', class: 'tile-class' }
  ];

  // Bar Chart 
  barChartLabels: Label[] = ['Th 1/2025', 'Th 2/2025', 'Th 3/2025', 'Th 4/2025', 'Th 5/2025', 'Th 6/2025'];
  barChartData: ChartDataSets[] = [
    { data: [17, 13, 17, 6, 14, 42], label: 'Lớp hoạt động', backgroundColor: '#0000ff' }
  ];
  barChartOptions: ChartOptions = {
    responsive: true,
    scales: {
      yAxes: [{
        ticks: {
          beginAtZero: true,
          stepSize: 5
        }
      }]
    }
  };
  barChartType: ChartType = 'bar';

  // Pie Chart 
  pieChartLabels: Label[] = ['Đặc biệt', 'Gia đình', 'Cơ bản'];
  pieChartData: number[] = [15, 20, 65];
  pieChartType: ChartType = 'pie';
  pieChartColors: Color[] = [
    {
      backgroundColor: ['#f44336', '#fdd835', '#4dd0e1']
    }
  ];
  pieChartOptions: ChartOptions = {
    responsive: true,
    legend: {
      position: 'right',
      labels: {
        fontSize: 13,
        fontColor: '#333'
      }
    }
  };
  attendanceData = [
    { code:'HS111', tenHocSinh: 'Bùi Ngọc Dũng', soBuoiNghi: 5, tiLe: 50, lop: 'Lớp toán 10A3' },
    { code:'HS111', tenHocSinh: 'Bùi Ngọc Dũng', soBuoiNghi: 4, tiLe: 40, lop: 'Lớp anh văn 10A3' },
    {code:'HS111', tenHocSinh: 'Bùi Ngọc Dũng', soBuoiNghi: 3, tiLe: 30, lop: 'Lớp văn 10A3' },
    {code:'HS111', tenHocSinh: 'Bùi Ngọc Dũng', soBuoiNghi: 3, tiLe: 30, lop: 'Lớp sử 10A3' },
    {code:'HS111', tenHocSinh: 'Bùi Ngọc Dũng', soBuoiNghi: 2, tiLe: 20, lop: 'Lớp toán tin 10A3' },
    { code:'HS111',tenHocSinh: 'Bùi Ngọc Dũng', soBuoiNghi: 2, tiLe: 20, lop: 'Lớp toán 10A3' }
  ];
  
  roomUsageList = [
    { name: '104', used: 10 },
    { name: '103', used: 8 },
    { name: '102', used: 9 },
    { name: '101', used: 7 },
    { name: '100', used: 6 },
    { name: '105', used: 5 },
    { name: '106', used: 9 },
    { name: '107', used: 4 },
    { name: '108', used: 3 },
    { name: '109', used: 18}
  ];

  doughnutLabels: Label[] = ['Có mặt', 'Vắng'];
doughnutData: number[] = [85, 15];
doughnutColors: Color[] = [
  { backgroundColor: ['#4dd0e1', '#f44336'] }
];
doughnutOptions: ChartOptions = {
  responsive: true,
  maintainAspectRatio: true,
  cutoutPercentage: 60,
  legend: {
    position: 'bottom',
    labels: {
      fontSize: 13,
      fontColor: '#333'
    }
  }
};

classAttendanceList = [
  { name: 'Lớp anh văn I', percentage: 85, totalSessions: 10 },
  { name: 'Lớp anh văn II', percentage: 80, totalSessions: 10 },
  { name: 'Lớp anh văn III', percentage: 75, totalSessions: 10 },
  { name: 'Lớp anh văn IV', percentage: 90, totalSessions: 10 },
  { name: 'Lớp anh văn V', percentage: 60, totalSessions: 10 },
  { name: 'Lớp anh văn VI', percentage: 88, totalSessions: 10 }
];

  constructor() { }
  
  ngOnInit(): void {
  }
}
