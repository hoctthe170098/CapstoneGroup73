import { Component, OnInit } from '@angular/core';
import { ChartOptions, ChartType, ChartDataSets } from 'chart.js';
import { Label, Color } from 'ng2-charts';

@Component({
  selector: 'app-dashboard-admin',
  templateUrl: './dashboard-admin.component.html',
  styleUrls: ['./dashboard-admin.component.scss']
})
export class DashboardAdminComponent implements OnInit {
  tiles = [
    { label: 'Số học sinh', value: 200, icon: 'ft-user', class: 'tile-student' },
    { label: 'Số giáo viên', value: 30, icon: 'ft-briefcase', class: 'tile-teacher' },
    { label: 'Số nhân viên', value: 15, icon: 'ft-bar-chart', class: 'tile-employee' }
  ];

  tiles2 = [
    { label: 'Số lớp học', value: 100, icon: 'ft-user' },
    { label: 'Số lớp đang diễn ra', value: 80, icon: 'ft-user' }
  ];

  barChartLabels: Label[] = ['Hoàng Văn Thái', 'Trần Duy Hưng', 'Cầu Giấy', 'Lê Trọng Tấn', 'Xã Đàn'];
  barChartData: ChartDataSets[] = [
    {
      data: [17, 13, 17, 6, 14],
      label: 'Học sinh',
      backgroundColor: '#0000ff',
      hoverBackgroundColor: '#0000ff',
      barPercentage: 0.8,
      categoryPercentage: 0.5
    },
    {
      data: [12, 10, 11, 5, 10],
      label: 'Giáo viên',
      backgroundColor: '#00cccc',
      hoverBackgroundColor: '#00cccc',
      barPercentage: 0.8,
      categoryPercentage: 0.5
    }
  ];
  barChartOptions: ChartOptions = {
    responsive: true,
    maintainAspectRatio: false,
    scales: {
      xAxes: [{
        gridLines: { display: false },
        ticks: {
          fontSize: 12,
          maxRotation: 0,
          minRotation: 0
        }
      }],
      yAxes: [{
        ticks: {
          beginAtZero: true,
          stepSize: 5,
          fontSize: 12
        },
        gridLines: { color: '#eee' }
      }]
    },
    legend: {
      position: 'top',
      labels: {
        fontSize: 13,
        fontColor: '#333',
        usePointStyle: true
      }
    },
    tooltips: { enabled: true }
  };

  pieChartLabels: Label[] = ['Đặc biệt', 'Gia đình', 'Cơ bản'];
  pieChartData: number[] = [15, 20, 65];
  pieChartColors: Color[] = [
    { backgroundColor: ['#f44336', '#fdd835', '#4dd0e1'] }
  ];
  pieChartOptions: ChartOptions = {
    responsive: true,
    maintainAspectRatio: true,
    legend: {
      position: 'right',
      labels: {
        fontSize: 13,
        fontColor: '#333'
      }
    }
  };

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

  horizontalLabels: Label[] = ['Hoàng Văn Thái', 'Trần Duy Hưng', 'Cầu Giấy', 'Lê Trọng Tấn', 'Xã Đàn', 'Hoàng Hoa Thám','Long Biên','Hoàng Mai', 'Đông Anh', 'a', 'b', 'c', 'd', 'e', 'f','g','h','i','k'];
  horizontalData: ChartDataSets[] = [
    {
      data: [16, 12, 11, 10, 9, 8, 7,6,5,1,2,3,4,5,6,7,8,9,10,11],
      label: 'Lớp',
      backgroundColor: '#ff9800'
    } 
  ];
  horizontalOptions: ChartOptions = {
    responsive: true,
    maintainAspectRatio: false,
    scales: {
      xAxes: [{
        ticks: { beginAtZero: true, stepSize: 3 },
        gridLines: { color: '#eee' }
      }],
      yAxes: [{
        gridLines: { color: '#eee' },
        ticks: { fontSize: 12 }
      }]
    },
    legend: { display: false }
  };

  constructor() { }

  ngOnInit(): void { }
  
  
  
}
