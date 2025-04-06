import { Component, OnInit } from '@angular/core';
import { LichHocService } from './shared/lichday.service';
import { LichHocTrongNgay } from './shared/lichday.model';
import { ChangeDetectorRef } from '@angular/core';
@Component({
  selector: 'app-lichday',
  templateUrl: './lichday.component.html',
  styleUrls: ['./lichday.component.scss']
})
export class LichDayComponent implements OnInit {
  hours = Array.from({ length: 15 }, (_, i) => i + 8); // 8h - 22h
  weeks = Array.from({ length: 52 }, (_, i) => `Tuần ${i + 1}`);
  years = [2023, 2024, 2025];

  selectedWeek = new Date().getWeekNumber();
  selectedYear = new Date().getFullYear();

  schedule: LichHocTrongNgay[] = [];

  constructor(private lichHocService: LichHocService,private cdr: ChangeDetectorRef) {}

  ngOnInit(): void {
    this.loadSchedule();
  }

  loadSchedule() {
    this.lichHocService.getLichHocGiaoVien(this.selectedWeek, this.selectedYear).subscribe(res => {
      if (!res.isError) {
        this.schedule = res.data.lichHocCaTuans;
        this.cdr.detectChanges();
      }
    });
  }

  onWeekChange(weekLabel: string) {
    this.selectedWeek = parseInt(weekLabel.replace('Tuần ', ''), 10);
    this.loadSchedule();
  }

  onYearChange(year: number) {
    this.selectedYear = year;
    this.loadSchedule();
  }

  getLessonStyle(lesson: { gioBatDau: string; gioKetThuc: string }) {
    const parseTime = (t: string) => {
      const [h, m] = t.split(':').map(Number);
      return h + m / 60;
    };

    const start = parseTime(lesson.gioBatDau);
    const end = parseTime(lesson.gioKetThuc);

    const top = (start - 8) * 60;
    const height = (end - start) * 60;

    return {
      top: `${top}px`,
      height: `${height}px`
    };
  }
  formatTime(timeStr: string): string {
    const date = new Date(`1970-01-01T${timeStr}`);
    const hours = date.getHours().toString().padStart(2, '0');
    const minutes = date.getMinutes().toString().padStart(2, '0');
    return `${hours}:${minutes}`;
  }
  
}

// Utility: Thêm method getWeekNumber vào Date prototype
declare global {
  interface Date {
    getWeekNumber(): number;
  }
}

Date.prototype.getWeekNumber = function () {
  const date = new Date(Date.UTC(this.getFullYear(), this.getMonth(), this.getDate()));
  const dayNum = date.getUTCDay() || 7;
  date.setUTCDate(date.getUTCDate() + 4 - dayNum);
  const yearStart = new Date(Date.UTC(date.getUTCFullYear(), 0, 1));
  return Math.ceil((((date.getTime() - yearStart.getTime()) / 86400000) + 1) / 7);
};

