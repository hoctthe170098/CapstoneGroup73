import { Component, OnInit, OnDestroy, ChangeDetectorRef } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import { saveAs } from 'file-saver';
import { LopdanghocService } from 'app/lopdanghoc/shared/lopdanghoc.service';
import { ToastrService } from 'ngx-toastr';
import { ViewChild } from '@angular/core';
import { QuillEditorComponent } from 'ngx-quill';

@Component({
  selector: 'app-chitiet-baitaphocsinh',
  templateUrl: './chitiet-baitaphocsinh.component.html',
  styleUrls: ['./chitiet-baitaphocsinh.component.scss']
})
export class ChitietBaitaphocsinhComponent implements OnInit, OnDestroy {
  baiTapId: string = '';
  baiTapDetail: any = null;
  countdownDisplay: string = '';
  intervalId: any;
  newAnswer = { noiDung: '', file: null };
  currentUserName = '';
  editingAnswerId: string | null = null;
  editAnswerData = {
    noiDung: '',
    file: null as null | { ten: string; url?: string; kichThuoc?: string; rawFile?: File }
  };
  @ViewChild('quillEditor') quillEditor!: QuillEditorComponent;
  @ViewChild('createFileInput') createFileInput!: HTMLInputElement;
  @ViewChild('editFileInput') editFileInput!: HTMLInputElement;
  @ViewChild('quillEditorEdit') quillEditorEdit!: QuillEditorComponent;
  answers: any[] ;
  constructor(
    private route: ActivatedRoute,
    private lopdanghocService: LopdanghocService,
    private cdr: ChangeDetectorRef, 
    private toastr: ToastrService
  ) {}

  ngOnInit(): void {
    this.route.paramMap.subscribe(params => {
      this.baiTapId = params.get('baiTapId') || '';
      if (this.baiTapId) {
        this.loadBaiTapDetail();
        this.loadTraLoi();
      }
    });
  }

  ngOnDestroy(): void {
    if (this.intervalId) clearInterval(this.intervalId);
  }
  quillModules = {
    toolbar: [
      ['bold', 'italic', 'underline', 'strike'],         
      ['blockquote', 'code-block'],                      
      [{ 'header': 1 }, { 'header': 2 }],                
      [{ 'list': 'ordered' }, { 'list': 'bullet' }],   
      [{ 'script': 'sub' }, { 'script': 'super' }],      
      [{ 'align': [] }],                                
      [{ 'header': [1, 2, 3, 4, 5, 6, false] }],          
      [{ 'size': ['small', false, 'large', 'huge'] }],   
      [{ 'font': [] }],                                  
      [{ 'color': [] }, { 'background': [] }],           
      ['clean']                                         
    ]
  };
  
  loadBaiTapDetail(): void {
    this.lopdanghocService.getBaiTapDetailForStudent(this.baiTapId).subscribe({
      next: (res) => {
        if (!res.isError) {
          this.baiTapDetail = res.data;
          this.startCountdown(res.data.secondsUntilDeadline);
        }
      },
      error: (err) => {
        console.error('Lỗi lấy chi tiết bài tập:', err);
      }
    });
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
        this.loadTraLoi();
        
        return;
      }
  
      this.updateCountdownDisplay(remaining);
      this.cdr.detectChanges();
    }, 1000);
  }
  
  

  updateCountdownDisplay(seconds: number): void {
    const days = Math.floor(seconds / (3600 * 24));
    const hours = Math.floor((seconds % (3600 * 24)) / 3600);
    const minutes = Math.floor((seconds % 3600) / 60);
    const secs = seconds % 60;
    this.countdownDisplay = `${days}N ${this.pad(hours)}:${this.pad(minutes)}:${this.pad(secs)}`;
  }

  pad(n: number): string {
    return n.toString().padStart(2, '0');
  }

  downloadFile(filePath?: string): void {
    const path = filePath || this.baiTapDetail?.urlFile;
    if (!path) return;
  
  
    this.lopdanghocService.downloadBaiTapFile(path).subscribe({
      next: (res) => {
        
        if (res?.data?.fileContents) {
          const byteArray = new Uint8Array(
            atob(res.data.fileContents).split('').map(char => char.charCodeAt(0))
          );
          const blob = new Blob([byteArray], { type: res.data.contentType });
          const fileName = res.data.fileDownloadName || 'file_taive';
          saveAs(blob, fileName);
        } else {
          console.warn('Không có fileContents trong response');
        }
      },
      error: (err) => {
        console.error(' Lỗi khi gọi API tải file:', err);
      }
    });
  }
  
  

  getFileName(path: string): string {
    return path.split('/').pop() || 'Tệp đính kèm';
  }  

  handleFileUpload(event: any): void {
    const file = event.target.files[0];
    if (file) {
      this.newAnswer.file = file;
    }
  }
  handleEditFileUpload(event: any): void {
    const file = event.target.files[0];
    if (file) {
      this.editAnswerData.file = {
        ten: file.name,
        kichThuoc: `${Math.round(file.size / 1024)} KB`,
        rawFile: file
      };
      
    }
  }
  

 
  submitAnswer(): void {
    const htmlContent = this.quillEditor.quillEditor.root.innerHTML.trim();
    if (!htmlContent || htmlContent === '<p><br></p>') {
      this.toastr.warning('Vui lòng nhập nội dung trước khi gửi.');
      return;
    }
    
  
    const formData = new FormData();
    formData.append('baiTapId', this.baiTapId);
    formData.append('noiDung', htmlContent); 
    if (this.newAnswer.file) {
      formData.append('tepDinhKem', this.newAnswer.file);
    }
  
    this.lopdanghocService.createTraLoi(formData).subscribe({
      next: (res) => {
        if (!res.isError) {
          this.toastr.success(res.message || 'Gửi câu trả lời thành công!');
  
          const newTraLoi = res.data;
  
          this.answers.unshift({
            tenHocSinh: this.currentUserName,
            thoiGianNop: new Date(newTraLoi.thoiGian).toLocaleTimeString([], {
              hour: '2-digit',
              minute: '2-digit',
            }),
            noiDung: htmlContent,
            file: newTraLoi.urlFile
              ? {
                  ten: this.getFileName(newTraLoi.urlFile),
                  kichThuoc: '---',
                }
              : null,
              isEditing: false 
          });
  
          // Reset
          this.quillEditor.quillEditor.setText('');
          this.newAnswer = { noiDung: '', file: null };
          this.cdr.detectChanges();
          this.loadTraLoi();
        } else {
          this.toastr.error(res.message || 'Gửi thất bại!');
        }
      },
      error: (err) => {
        console.error(' Lỗi gửi trả lời:', err);
        this.toastr.error('Đã xảy ra lỗi khi gửi câu trả lời!');
      }
    });
  }
  loadTraLoi(): void {
    this.lopdanghocService.getTraLoiByBaiTapForStudent(this.baiTapId).subscribe({
      next: (res) => {
        if (!res.isError) {
          this.answers = res.data.map((item: any) => ({
            id: item.id,
            tenHocSinh: item.hocSinhTen,
            thoiGianNop: new Date(item.thoiGian).toLocaleTimeString([], { hour: '2-digit', minute: '2-digit' }),
            noiDung: item.noiDung,
            diem: item.diem,
            nhanXet: item.nhanXet,
            file: item.urlFile
              ? {
                  ten: this.getFileName(item.urlFile),
                  url: item.urlFile
                }
              : null,
              isEditing: false 
          }));
        }
      },
      error: (err) => {
        console.error(' Lỗi lấy danh sách trả lời:', err);
      }
    });
  }
  deleteAnswer(ans: any): void {
    if (!ans.id) {
      this.toastr.error('Không xác định được câu trả lời để xoá.');
      return;
    }
  
    if (!confirm('Bạn có chắc muốn xoá câu trả lời này?')) return;
  
    this.lopdanghocService.deleteTraLoi(ans.id).subscribe({
      next: (res) => {
        if (!res.isError) {
          this.toastr.success(res.message || 'Xoá câu trả lời thành công!');
          // Gỡ câu trả lời ra khỏi danh sách hiện tại
          this.answers = this.answers.filter(item => item.id !== ans.id);
          this.cdr.detectChanges();
        } else {
          this.toastr.error(res.message || 'Xoá thất bại!');
        }
      },
      error: (err) => {
        console.error(' Lỗi xoá câu trả lời:', err);
        this.toastr.error('Đã xảy ra lỗi khi xoá câu trả lời!');
      }
    });
  }
  editAnswer(ans: any): void {
    this.editingAnswerId = ans.id;
    this.editAnswerData = {
      noiDung: ans.noiDung,
      file: ans.file
        ? {
            ten: ans.file.ten,
            url: ans.file.url,
            kichThuoc: ans.file.kichThuoc
          }
        : null
    };
  }
  
  
  
  
  
  
  cancelAnswer(): void {
    this.newAnswer = { noiDung: '', file: null };
  }
  removeCreateFile(): void {
    this.newAnswer.file = null;
    if (this.createFileInput) {
      this.createFileInput.value = '';
    }
  }
  
  removeEditFile(): void {
    this.editAnswerData.file = null;
    if (this.editFileInput) {
      this.editFileInput.value = '';
    }
  }
  cancelEdit(): void {
    this.editingAnswerId = null;
    this.editAnswerData = {
      noiDung: '',
      file: null
    };
  }
  submitEditedAnswer(traLoiId: string): void {
    const htmlContent = this.editAnswerData.noiDung?.trim();
  
    if (!htmlContent || htmlContent === '<p><br></p>') {
      this.toastr.warning('Vui lòng nhập nội dung trước khi lưu.');
      return;
    }
  
    const formData = new FormData();
    formData.append('traLoiId', traLoiId);
    formData.append('noiDung', htmlContent);
  
  
    if (this.editAnswerData.file?.rawFile) {
      formData.append('tepDinhKemMoi', this.editAnswerData.file.rawFile);
    }
  
    this.lopdanghocService.updateTraLoi(formData).subscribe({
      next: (res) => {
        if (!res.isError) {
          this.toastr.success(res.message || 'Cập nhật câu trả lời thành công!');
          this.editingAnswerId = null;
          this.editAnswerData = { noiDung: '', file: null };
          this.loadTraLoi(); // Load lại để lấy đúng dữ liệu từ BE
        } else {
          this.toastr.error(res.message || 'Cập nhật thất bại!');
        }
      },
      error: (err) => {
        console.error('❌ Lỗi cập nhật:', err);
        this.toastr.error('Đã xảy ra lỗi khi cập nhật!');
      }
    });
  }
  
  
  
}

