import { Component } from '@angular/core';

@Component({
  selector: 'app-slot',
  templateUrl: './slot.component.html',
  styleUrls: ['./slot.component.scss']
})
export class SlotComponent {
  // Dữ liệu mẫu
  slots = [
    { name: 'Slot 1', startTime: '07:30 AM', endTime: '09:30 AM' },
    { name: 'Slot 2', startTime: '09:30 AM', endTime: '11:30 AM' },
    { name: 'Slot 3', startTime: '01:00 PM', endTime: '03:00 PM' },
  ];

  // Tham chiếu form
  slotName: string = '';
  startTime: string = '';
  endTime: string = '';

  addSlot() {
    if (this.slotName && this.startTime && this.endTime) {
      this.slots.push({
        name: this.slotName,
        startTime: this.startTime,
        endTime: this.endTime
      });
      this.resetForm();
    }
  }

  resetForm() {
    this.slotName = '';
    this.startTime = '';
    this.endTime = '';
  }

  deleteSlot(index: number) {
    this.slots.splice(index, 1);
  }

  editSlot(index: number) {
    alert('Sửa slot index = ' + index);
  }
}
