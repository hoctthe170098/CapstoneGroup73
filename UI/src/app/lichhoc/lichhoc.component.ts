import { Component, OnInit, ChangeDetectorRef } from '@angular/core';
import { LichHocService } from './shared/lichhoc.service'; // sửa lại path nếu khác
import { GetLichHocHocSinhResponse, LichHocTrongNgay } from './shared/lichhoc.model';
import { Router } from "@angular/router";
@Component({
  selector: 'app-lichhoc',
  templateUrl: './lichhoc.component.html',
  styleUrls: ['./lichhoc.component.scss']
})
export class LichhocComponent implements OnInit {
  hours = Array.from({ length: 15 }, (_, i) => i + 8); // từ 8h đến 22h
  years: number[] = [];
  weeks: { value: number; label: string }[] = [];
  selectedWeek = new Date().getWeekNumber();
  selectedYear = new Date().getFullYear();
  schedule: LichHocTrongNgay[] = [];

  constructor(
    private lichHocService: LichHocService,
    private cdr: ChangeDetectorRef,
    private router:Router
  ) {}

  ngOnInit(): void {
    const currentYear = new Date().getFullYear();
    this.years = [currentYear - 1, currentYear, currentYear + 1];
    this.generateWeeks();
    this.loadSchedule();
  }

  generateWeeks() {
    this.weeks = Array.from({ length: 52 }, (_, i) => {
      const weekNumber = i + 1;
      const startDate = this.getDateOfISOWeek(weekNumber, this.selectedYear);
      const endDate = new Date(startDate);
      endDate.setDate(startDate.getDate() + 6);

      const format = (d: Date) =>
        `${d.getDate().toString().padStart(2, '0')}/${(d.getMonth() + 1)
          .toString()
          .padStart(2, '0')}`;

      return {
        value: weekNumber,
        label: `(${format(startDate)} - ${format(endDate)})`
      };
    });
  }

  getDateOfISOWeek(week: number, year: number): Date {
    const simple = new Date(year, 0, 1 + (week - 1) * 7);
    const dow = simple.getDay();
    const ISOweekStart = new Date(simple);
    if (dow <= 4)
      ISOweekStart.setDate(simple.getDate() - simple.getDay() + 1);
    else
      ISOweekStart.setDate(simple.getDate() + 8 - simple.getDay());
    return ISOweekStart;
  }

  loadSchedule() {
    this.lichHocService
      .getLichHocHocSinh(this.selectedWeek, this.selectedYear)
      .subscribe((res) => {
        if (res.code === 404) {
          this.router.navigate(['/pages/error'])
          return;
        }
        if (!res.isError) {
          this.schedule = res.data.lichHocCaTuans.map((day) => {
            const date = new Date(day.ngay);
            const shortDays = ['CN', 'T2', 'T3', 'T4', 'T5', 'T6', 'T7'];
            const dayIndex = date.getDay();
            return {
              ...day,
              thuRutGon: `${shortDays[dayIndex]} ${date
                .getDate()
                .toString()
                .padStart(2, '0')}/${(date.getMonth() + 1)
                .toString()
                .padStart(2, '0')}`
            };
          });
          this.cdr.detectChanges();
        }
      });
  }

  onWeekChange() {
    this.loadSchedule();
  }

  onYearChange(year: number) {
    this.selectedYear = year;
    this.generateWeeks();
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

  formatTrangThai(status: string): string {
    return status === 'Dạy bù' ? 'Học bù' : status;
  }
}

// Thêm phương thức getWeekNumber cho Date
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
