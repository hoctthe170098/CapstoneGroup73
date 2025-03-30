import { Component } from '@angular/core';

@Component({
  selector: 'app-lichday',
  templateUrl: './lichday.component.html',
  styleUrls: ['./lichday.component.scss']
})
export class LichDayComponent {
  // Danh sách giờ trong ngày (8h đến 22h)
  hours = Array.from({ length: 15 }, (_, i) => i + 8); // [8, 9, ..., 20]

  // Dropdown Tuần & Năm
  weeks = ['Tuần 1', 'Tuần 2', 'Tuần 3', 'Tuần 4'];
  years = [2024, 2025, 2026];

  selectedWeek = 'Tuần 1';
  selectedYear = 2024;

  // Lịch mẫu cho mỗi tuần
  allSchedules: Record<string, any[]> = {
    'Tuần 1': [
      {
        label: 'Th 2 24/3',
        lessons: [
          { title: 'Toán 9 - Ôn tập', room: 'Phòng DE.102', time: '8:30 - 10:30' },
          { title: 'Toán 9 - Ôn tập', room: 'Phòng DE.102', time: '14:30 - 16:30' }
        ]
      },
      {
        label: 'Th 3 25/3',
        lessons: [
          { title: 'Toán 9 - Ôn tập', room: 'Phòng DE.102', time: '9:00 - 10:00' },
          { title: 'Toán 9 - Ôn tập', room: 'Phòng DE.102', time: '17:00 - 18:00' }
        ]
      },
      {
        label: 'Th 4 26/3',
        lessons: [
          { title: 'Toán 9 - Ôn tập', room: 'Phòng DE.102', time: '10:00 - 12:00' }
        ]
      },
      { label: 'Th 5 27/3', lessons: [] },
      {
        label: 'Th 6 28/3',
        lessons: [
          { title: 'Toán 9 - Ôn tập', room: 'Phòng DE.102', time: '8:00 - 9:30' },
          { title: 'Toán 9 - Ôn tập', room: 'Phòng DE.102', time: '15:00 - 16:30' }
        ]
      },
      {
        label: 'Th 7 29/3',
        lessons: [
          { title: 'Toán 9 - Ôn tập', room: 'Phòng DE.102', time: '8:00 - 10:00' }
        ]
      },
      {
        label: 'CN 30/3',
        lessons: [
          { title: 'Toán 9 - Ôn tập', room: 'Phòng DE.102', time: '10:30 - 12:00' }
        ]
      }
    ],
    'Tuần 2': [
      // Tuần 2 dữ liệu khác nếu muốn
      { label: 'Th 2 31/3', lessons: [] },
      { label: 'Th 3 1/4', lessons: [] },
      { label: 'Th 4 2/4', lessons: [] },
      { label: 'Th 5 3/4', lessons: [] },
      { label: 'Th 6 4/4', lessons: [] },
      { label: 'Th 7 5/4', lessons: [] },
      { label: 'CN 6/4', lessons: [] }
    ]
  };

  // Lịch được hiển thị theo tuần
  schedule = this.allSchedules[this.selectedWeek];

  constructor() {}

  ngOnInit(): void {
    this.loadSchedule();
  }

  // Load lại lịch khi chọn tuần/năm
  loadSchedule() {
    this.schedule = this.allSchedules[this.selectedWeek] || [];
    // Nếu fetch từ API: gọi API tại đây với selectedWeek, selectedYear
  }

  // Tự động load lại nếu chọn thay đổi
  onWeekChange(week: string) {
    this.selectedWeek = week;
    this.loadSchedule();
  }

  onYearChange(year: number) {
    this.selectedYear = year;
    this.loadSchedule();
  }

  // Tính style (top, height) cho mỗi buổi học dựa trên giờ
  getLessonStyle(lesson: { time: string }) {
    const [start, end] = lesson.time.split(' - ').map(t => {
      const [h, m] = t.split(':').map(Number);
      return h + m / 60;
    });

    const top = (start - 8) * 60; // Bắt đầu từ 8h, mỗi giờ 60px
    const height = (end - start) * 60;

    return {
      top: `${top}px`,
      height: `${height}px`
    };
  }
}
