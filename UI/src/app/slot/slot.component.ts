import { Component, OnInit, ChangeDetectorRef } from '@angular/core';
import { SlotService } from './shared/slot.service';
import { Slot } from './shared/slot.model';
import { ToastrService } from 'ngx-toastr';

@Component({
  selector: 'app-slot',
  templateUrl: './slot.component.html',
  styleUrls: ['./slot.component.scss']
})
export class SlotComponent implements OnInit {
  // Danh sách phòng
  rooms: Slot[] = [];
  // Form thêm
  roomName: string = '';

  // Popup edit
  isEditModalOpen: boolean = false;
  editRoomId: number | null = null;
  editRoomName: string = '';
  editRoomStatus: string = 'Mở'; // UI: "Mở" hoặc "Đóng"

  // Biến cờ để kiểm soát hiển thị lỗi sau khi bấm nút
  isSubmittedAdd: boolean = false;
  isSubmittedEdit: boolean = false;

  constructor(
    private slotService: SlotService,
    private cd: ChangeDetectorRef,
    private toastr: ToastrService
  ) {}

  ngOnInit(): void {
    this.loadRooms();
  }

  // Lấy danh sách phòng
  loadRooms(): void {
    this.slotService.getDanhSachPhong().subscribe({
      next: (res) => {
        if (!res.isError && res.data) {
          // Chuyển "use"/"nonuse" -> "Mở"/"Đóng"
          this.rooms = res.data.map((r: any) => ({
            ...r,
            trangThai: this.translateStatus(r.trangThai)
          }));
          this.cd.detectChanges();
        } else {
          this.toastr.error(res.message || 'Có lỗi xảy ra.');
        }
      },
      error: (err) => {
        console.error('Error fetching rooms:', err);
        this.toastr.error('Không thể tải danh sách phòng. Vui lòng thử lại!');
      }
    });
  }

  // "use" -> "Mở", "nonuse" -> "Đóng"
  translateStatus(status: string): string {
    if (status === 'use') return 'Mở';
    if (status === 'nonuse') return 'Đóng';
    return status;
  }

  // Kiểm tra trùng tên (cho form thêm)
  isDuplicateRoomName(name: string): boolean {
    const trimmed = name.trim().toLowerCase();
    return this.rooms.some(r => r.ten.toLowerCase() === trimmed);
  }

  // Kiểm tra trùng tên (cho form edit) - bỏ qua phòng đang edit
  isDuplicateEditRoomName(name: string): boolean {
    const trimmed = name.trim().toLowerCase();
    return this.rooms.some(r =>
      r.id !== this.editRoomId && r.ten.toLowerCase() === trimmed
    );
  }

  // Thêm phòng
  addRoom(): void {
    this.isSubmittedAdd = true; // người dùng đã bấm nút => cho phép hiển thị lỗi
    const nameTrim = this.roomName.trim();

    // Validate
    if (!nameTrim) {
      return; // Tên rỗng => dừng
    }
    if (nameTrim.length > 20) {
      return; // Quá 20 ký tự => dừng
    }
    if (this.isDuplicateRoomName(nameTrim)) {
      return; // Tên đã tồn tại => dừng
    }

    // Gọi API
    const body = { ten: nameTrim };
    this.slotService.createPhong(body).subscribe({
      next: (res) => {
        if (!res.isError && res.data) {
          // Thêm phòng vào rooms
          const newRoom = {
            ...res.data,
            trangThai: this.translateStatus(res.data.trangThai)
          };
          this.rooms.push(newRoom);

          // Reset form
          this.roomName = '';
          this.isSubmittedAdd = false;
          this.loadRooms();
          this.toastr.success('Tạo phòng thành công!');
        } else {
          this.toastr.error(res.message || 'Không thể tạo phòng.');
        }
      },
      error: (err) => {
        console.error('Error creating room:', err);
        this.toastr.error('Có lỗi xảy ra khi tạo phòng!');
      }
    });
  }

  // Mở pop-up Edit
  openEditRoomModal(roomId: number): void {
    const room = this.rooms.find(r => r.id === roomId);
    if (room) {
      this.editRoomId = room.id;
      this.editRoomName = room.ten;
      this.editRoomStatus = room.trangThai; // "Mở" hoặc "Đóng"
      this.isEditModalOpen = true;
      this.isSubmittedEdit = false; // reset cờ
      this.cd.detectChanges();
    }
  }

  // Áp dụng Edit
  applyEditRoom(): void {
    this.isSubmittedEdit = true; // user đã bấm => hiển thị lỗi
    const nameTrim = this.editRoomName.trim();

    // Validate
    if (!nameTrim) {
      return; // Tên rỗng => dừng
    }
    if (nameTrim.length > 20) {
      return; // Quá 20 ký tự => dừng
    }
    if (this.isDuplicateEditRoomName(nameTrim)) {
      return; // Tên đã tồn tại => dừng
    }

    const confirmResult = window.confirm('Bạn có chắc muốn thay đổi thông tin phòng này?');
    if (!confirmResult) {
      this.isEditModalOpen = false;
      return;
    }else this.isEditModalOpen = false;

    // Chuyển UI -> API: "Mở" -> "use", "Đóng" -> "nonuse"
    const apiStatus = (this.editRoomStatus === 'Mở') ? 'use' : 'nonuse';
    const data: Slot = {
      id: this.editRoomId!,
      ten: nameTrim,
      trangThai: apiStatus
    };

    this.slotService.updatePhong(data).subscribe({
      next: (res) => {
        if (!res.isError && res.data) {
          // Cập nhật rooms
          const idx = this.rooms.findIndex(r => r.id === this.editRoomId);
          if (idx !== -1) {
            this.rooms[idx].ten = nameTrim;
            this.rooms[idx].trangThai = this.translateStatus(res.data.trangThai);
          }
          this.isEditModalOpen = false; 
          this.toastr.success('Chỉnh sửa phòng thành công!');
        } else {
          this.toastr.error(res.message || 'Không thể chỉnh sửa phòng.');
        }
      },
      error: (err) => {
        console.error('Error updating room:', err);
        this.toastr.error('Có lỗi xảy ra khi chỉnh sửa phòng!');
      }
    });
  }
  // Bấm nút "Sửa" trong bảng
  editRoom(roomId: number): void {
    this.openEditRoomModal(roomId);
  }
}
