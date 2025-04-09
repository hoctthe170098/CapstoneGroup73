import { ChangeDetectorRef, Component, OnInit } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import { LopdangdayService } from 'app/lopdangday/shared/lopdangday.service';
import { saveAs } from 'file-saver';
import { ToastrService } from 'ngx-toastr';

@Component({
  selector: 'app-chitiet-baitap',
  templateUrl: './chitiet-baitap.component.html',
  styleUrls: ['./chitiet-baitap.component.scss']
})
export class ChitietBaitapComponent implements OnInit {

  tenLop: string = '';
  baiTapId: string = '';

  baiTapDetail: any = null;
  isLoading: boolean = false;
  isEditModalOpen = false;

  intervalId: any;
countdownDisplay: string = '';


  ngOnInit(): void {
    this.route.paramMap.subscribe(params => {
      this.tenLop = decodeURIComponent(params.get('tenLop') || '');
      this.baiTapId = params.get('baiTapId') || '';

      if (this.baiTapId) {
        this.loadBaiTapDetail();
      }
    });
  }
  ngOnDestroy(): void {
    if (this.intervalId) {
      clearInterval(this.intervalId);
    }
  }
  
  constructor(
    private route: ActivatedRoute,
    private lopdangdayService: LopdangdayService,
    private cdr: ChangeDetectorRef,
    private toastr: ToastrService
  ) {}

  loadBaiTapDetail(): void {
    this.isLoading = true;
    this.lopdangdayService.getBaiTapDetail(this.baiTapId).subscribe({
      next: (res) => {
        if (!res.isError) {
          this.baiTapDetail = res.data;
          this.startCountdown(this.baiTapDetail.secondsUntilDeadline);
        }
        this.isLoading = false;
      },
      error: (err) => {
        console.error(' Lỗi lấy chi tiết bài tập:', err);
        this.isLoading = false;
      }
    });
  }

  formatSecondsToTime(totalSeconds: number): string {
    const days = Math.floor(totalSeconds / (3600 * 24));
    const hours = Math.floor((totalSeconds % (3600 * 24)) / 3600);
    const minutes = Math.floor((totalSeconds % 3600) / 60);
    const seconds = totalSeconds % 60;
  
    const pad = (n: number) => n.toString().padStart(2, '0');
    return `${days}N ${pad(hours)}:${pad(minutes)}:${pad(seconds)}`;
  }
  
  startCountdown(seconds: number) {
   
    if (this.intervalId) {
      clearInterval(this.intervalId);
    }
  
    let remaining = seconds;
    this.updateCountdownDisplay(remaining);
  
    this.intervalId = setInterval(() => {
      remaining--;
      this.updateCountdownDisplay(remaining);
      this.cdr.detectChanges();
      if (remaining <= 0) {
        clearInterval(this.intervalId);
      }
    }, 1000);
  }
  

updateCountdownDisplay(seconds: number) {
  this.countdownDisplay = this.formatSecondsToTime(seconds);
}
downloadFile(): void {
  if (!this.baiTapDetail?.urlFile) return;

  const filePath = this.baiTapDetail.urlFile;
  this.lopdangdayService.downloadBaiTapFile(filePath).subscribe({
    next: (res) => {
      if (res && res.data?.fileContents) {
        const byteCharacters = atob(res.data.fileContents);
        const byteNumbers = new Array(byteCharacters.length);
        for (let i = 0; i < byteCharacters.length; i++) {
          byteNumbers[i] = byteCharacters.charCodeAt(i);
        }
        const byteArray = new Uint8Array(byteNumbers);
        const blob = new Blob([byteArray], { type: res.data.contentType });

        const fileName = res.data.fileDownloadName || 'baitap.pdf';
        saveAs(blob, fileName); 
      }
    },
    error: (err) => {
      console.error('❌ Lỗi khi tải file:', err);
    }
  });
}

  answers = [
    {
      tenHocSinh: 'Bùi Ngọc Dũng',
      thoiGianNop: '18:30',
      noiDung: 'Lorem ipsum dolor sit amet, consectetur adipiscing elit...',
      file: {
        ten: 'File Title.pdf',
        kichThuoc: '313 KB',
       
      }
    },
    {
      tenHocSinh: 'Ngô Minh Kiên',
      thoiGianNop: '18:30',
      noiDung: 'Lorem ipsum dolor sit amet, consectetur adipiscing elit...',
      file: {
        ten: 'File Title.pdf',
        kichThuoc: '313 KB',
        
      }
    },
    {
      tenHocSinh: 'Nguyễn Tuấn Anh',
      thoiGianNop: '18:30',
      noiDung: 'Lorem ipsum dolor sit amet, consectetur adipiscing elit...',
      file: {
        ten: 'File Title.pdf',
        kichThuoc: '313 KB',
        
      }
    }
  ];
  editBaiTap: any;
  
  openEditModal(bt: any): void {
    console.log('🛠️ Dữ liệu bài tập cần sửa:', bt);
    this.editBaiTap = {
      tieuDe: bt.tieuDe,
      noiDung: bt.noiDung,
      gio: this.convertDateToDatetimeLocal(bt.thoiGianKetThuc),
      trangThai: bt.trangThai || '', 
      file: bt.urlFile
        ? {
            name: bt.urlFile.split('/').pop(),
            size: '' 
          }
        : null
    };
  
    this.isEditModalOpen = true;
  }
  
  convertDateToDatetimeLocal(dateString: string): string {
    const date = new Date(dateString);
    const offset = date.getTimezoneOffset();
    const localDate = new Date(date.getTime() - offset * 60000);
    return localDate.toISOString().slice(0, 16); // yyyy-MM-ddTHH:mm
  }
  
  closeEditModal() {
    this.isEditModalOpen = false;
  }
  
  onEditFileSelected(event: any): void {
    const file = event.target.files[0];
    if (file) {
      this.editBaiTap.file = {
        name: file.name,
        size: `${Math.round(file.size / 1024)} KB`,
        
      };
    }
  }
  
  removeEditFile(): void {
    this.editBaiTap.file = null;
  }
  
  confirmEdit(): void {
    const formData = new FormData();
  
    formData.append('updateBaiTapDto.id', this.baiTapId);
    formData.append('updateBaiTapDto.tenLop', this.tenLop);
    formData.append('updateBaiTapDto.tieuDe', this.editBaiTap.tieuDe);
    formData.append('updateBaiTapDto.noiDung', this.editBaiTap.noiDung);
    formData.append('updateBaiTapDto.thoiGianKetThuc', this.editBaiTap.gio);
  
    if (this.editBaiTap.trangThai) {
      formData.append('updateBaiTapDto.trangThai', this.editBaiTap.trangThai);
    }
  
    if (this.editBaiTap.file instanceof File) {
      formData.append('updateBaiTapDto.taiLieu', this.editBaiTap.file);
    }
  
    this.lopdangdayService.updateBaiTap(formData).subscribe({
      next: (res) => {
        if (!res.isError) {
          this.toastr.success(res.message || 'Cập nhật bài tập thành công!');
          this.loadBaiTapDetail(); 
          this.cdr.detectChanges(); 
          this.closeEditModal();
        } else {
          this.toastr.error(res.message || 'Cập nhật thất bại!');
        }
      },
      error: (err) => {
        this.toastr.error(err?.error?.message || 'Đã xảy ra lỗi khi cập nhật!');
      }
    });
  }
  
  
}
