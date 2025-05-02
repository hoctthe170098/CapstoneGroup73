import { Component, OnInit, ChangeDetectorRef } from '@angular/core';
import { SlotService } from './shared/slot.service';
import { Slot } from './shared/slot.model';
import { ToastrService } from 'ngx-toastr';
import { Router } from "@angular/router";
@Component({
  selector: 'app-slot',
  templateUrl: './slot.component.html',
  styleUrls: ['./slot.component.scss']
})
export class SlotComponent implements OnInit {
  
  rooms: Slot[] = [];

  roomName: string = '';

  
  isEditModalOpen: boolean = false;
  editRoomId: number | null = null;
  editRoomName: string = '';
  editRoomStatus: string = 'Mở'; 

  isSubmittedAdd: boolean = false;
  isSubmittedEdit: boolean = false;

  constructor(
    private slotService: SlotService,
    private cd: ChangeDetectorRef,
    private toastr: ToastrService,
    private router: Router
  ) {}

  ngOnInit(): void {
    this.loadRooms();
  }

 
  loadRooms(): void {
    this.slotService.getDanhSachPhong().subscribe({
      next: (res) => {
        if (res.code === 404) {
          this.router.navigate(['/pages/error'])
          return;
        }
        if (!res.isError && res.data) {
          
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

  
  translateStatus(status: string): string {
    if (status === 'use') return 'Mở';
    if (status === 'nonuse') return 'Đóng';
    return status;
  }

  
  isDuplicateRoomName(name: string): boolean {
    const trimmed = name.trim().toLowerCase();
    return this.rooms.some(r => r.ten.toLowerCase() === trimmed);
  }

  
  isDuplicateEditRoomName(name: string): boolean {
    const trimmed = name.trim().toLowerCase();
    return this.rooms.some(r =>
      r.id !== this.editRoomId && r.ten.toLowerCase() === trimmed
    );
  }

  
  addRoom(): void {
    this.isSubmittedAdd = true; // người dùng đã bấm nút => cho phép hiển thị lỗi
    const nameTrim = this.roomName.trim();

    
    if (!nameTrim) {
      return; 
    }
    if (nameTrim.length > 20) {
      return; 
    }
    if (this.isDuplicateRoomName(nameTrim)) {
      return; 
    }

    
    const body = { ten: nameTrim };
    this.slotService.createPhong(body).subscribe({
      next: (res) => {
        if (!res.isError && res.data) {
          
          const newRoom = {
            ...res.data,
            trangThai: this.translateStatus(res.data.trangThai)
          };
          this.rooms.push(newRoom);

          
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

 
  openEditRoomModal(roomId: number): void {
    const room = this.rooms.find(r => r.id === roomId);
    if (room) {
      this.editRoomId = room.id;
      this.editRoomName = room.ten;
      this.editRoomStatus = room.trangThai; 
      this.isEditModalOpen = true;
      this.isSubmittedEdit = false; 
      this.cd.detectChanges();
    }
  }

  // Edit
  applyEditRoom(): void {
    this.isSubmittedEdit = true; 
    const nameTrim = this.editRoomName.trim();

    // Validate
    if (!nameTrim) {
      return; 
    }
    if (nameTrim.length > 20) {
      return; 
    }
    if (this.isDuplicateEditRoomName(nameTrim)) {
      return; 
    }

    const confirmResult = window.confirm('Bạn có chắc muốn thay đổi thông tin phòng này?');
    if (!confirmResult) {
      this.isEditModalOpen = false;
      return;
    }else this.isEditModalOpen = false;

    const apiStatus = (this.editRoomStatus === 'Mở') ? 'use' : 'nonuse';
    const data: Slot = {
      id: this.editRoomId!,
      ten: nameTrim,
      trangThai: apiStatus
    };

    this.slotService.updatePhong(data).subscribe({
      next: (res) => {
        if (!res.isError && res.data) {
         
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

  editRoom(roomId: number): void {
    this.openEditRoomModal(roomId);
  }
}
