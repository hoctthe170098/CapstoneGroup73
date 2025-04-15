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
answers: any[] = [];
currentPage = 1;
pageSize = 3;
editBaiTap: any;
gradingAnswerId: string | null = null;
  ngOnInit(): void {
    this.route.paramMap.subscribe(params => {
      this.tenLop = decodeURIComponent(params.get('tenLop') || '');
      this.baiTapId = params.get('baiTapId') || '';

      if (this.baiTapId) {
        this.loadBaiTapDetail();
        this.loadTraLoiCuaHocSinh(); 
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
  
  startCountdown(seconds: number): void {
    if (this.intervalId) clearInterval(this.intervalId);
  
    let remaining = seconds;
    this.updateCountdownDisplay(remaining);
  
    this.intervalId = setInterval(() => {
      remaining--;
  
      if (remaining < 0) {
        clearInterval(this.intervalId);
        this.intervalId = null;
  
       
        this.loadBaiTapDetail();
        
        return;
      }
  
      this.updateCountdownDisplay(remaining);
      this.cdr.detectChanges();
    }, 1000);
  }
  
  

updateCountdownDisplay(seconds: number) {
  this.countdownDisplay = this.formatSecondsToTime(seconds);
}
downloadFile(filePath?: string): void {
  const path = filePath || this.baiTapDetail?.urlFile;
  if (!path) return;

  this.lopdangdayService.downloadBaiTapFile(path).subscribe({
    next: (res) => {
      if (res && res.data?.fileContents) {
        const byteCharacters = atob(res.data.fileContents);
        const byteNumbers = new Array(byteCharacters.length);
        for (let i = 0; i < byteCharacters.length; i++) {
          byteNumbers[i] = byteCharacters.charCodeAt(i);
        }
        const byteArray = new Uint8Array(byteNumbers);
        const blob = new Blob([byteArray], { type: res.data.contentType });

        const fileName = res.data.fileDownloadName || 'tep_dinh_kem.pdf';
        saveAs(blob, fileName);
      }
    },
    error: (err) => {
      console.error(' Lỗi khi tải file:', err);
    }
  });
}


  
  
  
  openEditModal(bt: any): void {
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
    return localDate.toISOString().slice(0, 16); 
  }
  
  closeEditModal() {
    this.isEditModalOpen = false;
  }
  
  onEditFileSelected(event: any, fileInput: HTMLInputElement): void {
    const file = event.target.files[0];
    if (file) {
      const validTypes = [
        'application/pdf',
        'application/msword',
        'application/vnd.openxmlformats-officedocument.wordprocessingml.document',
      ];
      const isValidType = validTypes.includes(file.type);
      const isValidSize = file.size <= 10 * 1024 * 1024;
  
      if (!isValidType) {
        this.toastr.error('Tệp không hợp lệ. Chỉ chấp nhận PDF, DOC hoặc DOCX.');
        return;
      }
  
      if (!isValidSize) {
        this.toastr.error('Tệp vượt quá dung lượng cho phép (tối đa 10MB).');
        return;
      }
  
      this.editBaiTap.file = {
        name: file.name,
        size: `${Math.round(file.size / 1024)} KB`,
        rawFile: file,
      };
  
      // Clear lỗi nếu có
      this.formErrorsEdit.file = '';
    }
  
    // Reset input để cho phép chọn lại cùng file
    if (fileInput) {
      fileInput.value = '';
    }
  }
  
  
  removeEditFile(): void {
    this.editBaiTap.file = null;
  }
  
  formErrorsEdit = {
    tieuDe: '',
    noiDung: '',
    thoiGianKetThuc: '',
    trangThai: '',
    file: ''
  };

  confirmEdit(): void {
  // Reset lỗi
  this.formErrorsEdit = { tieuDe: '', noiDung: '', thoiGianKetThuc: '', trangThai: '', file: '' };

  const { tieuDe, noiDung, gio, trangThai, file } = this.editBaiTap;

  // Validate
  if (!tieuDe?.trim()) {
    this.formErrorsEdit.tieuDe = 'Tiêu đề không được để trống.';
  } else if (tieuDe.length > 50) {
    this.formErrorsEdit.tieuDe = 'Tiêu đề tối đa 50 ký tự.';
  }

  if (!noiDung?.trim()) {
    this.formErrorsEdit.noiDung = 'Nội dung không được để trống.';
  } else if (noiDung.length > 750) {
    this.formErrorsEdit.noiDung = 'Nội dung tối đa 750 ký tự.';
  }

  if (!gio) {
    this.formErrorsEdit.thoiGianKetThuc = 'Thời gian kết thúc không được để trống.';
  } else if (new Date(gio) <= new Date()) {
    this.formErrorsEdit.thoiGianKetThuc = 'Thời gian kết thúc phải sau thời điểm hiện tại.';
  }

  const allowedStatus = ['Đang mở', 'Chưa mở', 'Kết thúc'];
  if (!trangThai) {
    this.formErrorsEdit.trangThai = 'Trạng thái không được để trống.';
  } else if (!allowedStatus.includes(trangThai)) {
    this.formErrorsEdit.trangThai = 'Trạng thái không hợp lệ.';
  }

  

  const hasError = Object.values(this.formErrorsEdit).some(e => e !== '');
  if (hasError) return;


  const formData = new FormData();
  formData.append('updateBaiTapDto.id', this.baiTapId);
  formData.append('updateBaiTapDto.tieuDe', tieuDe);
  formData.append('updateBaiTapDto.noiDung', noiDung);
  formData.append('updateBaiTapDto.thoiGianKetThuc', gio);
  formData.append('updateBaiTapDto.trangThai', trangThai);
  formData.append('updateBaiTapDto.tenLop', this.tenLop);

  if (file && file.rawFile) {
    formData.append('updateBaiTapDto.taiLieu', file.rawFile);
  }
  
 
  this.lopdangdayService.updateBaiTap(formData).subscribe({
    next: (res) => {
      if (!res.isError) {
        this.toastr.success(res.message || 'Cập nhật bài tập thành công!');
        this.closeEditModal();
        this.loadBaiTapDetail();
        this.cdr.detectChanges();
      } else {
        this.toastr.error(res.message || 'Cập nhật thất bại!');
      }
    },
    error: (err) => {
      this.toastr.error(err?.error?.message || 'Đã xảy ra lỗi khi cập nhật!');
    }
  });
}
loadTraLoiCuaHocSinh(): void {
  const payload = {
    baiTapId: this.baiTapId,
    pageNumber: this.currentPage,
    pageSize: this.pageSize,
  };

  this.lopdangdayService.getTraLoiByBaiTapForTeacher(payload).subscribe({
    next: (res) => {
      if (!res.isError) {
        const items = res.data?.items || [];
        this.answers = items.map((item: any) => ({
          tenHocSinh: item.hocSinhTen,
          thoiGianNop: new Date(item.thoiGian).toLocaleTimeString([], { hour: '2-digit', minute: '2-digit' }),
          noiDung: item.noiDung,
          file: item.urlFile
            ? {
                ten: 'File',
                url: item.urlFile,
              }
            : null
        }));
      } else {
        this.toastr.error(res.message || 'Không thể lấy danh sách trả lời.');
      }
    },
    error: (err) => {
      console.error(' Lỗi khi gọi API lấy trả lời học sinh:', err);
    }
  });
}
openChamDiemForm(ans: any): void {
  this.gradingAnswerId = ans.id;
}
cancelGrading(): void {
  this.gradingAnswerId = null;
}
}
