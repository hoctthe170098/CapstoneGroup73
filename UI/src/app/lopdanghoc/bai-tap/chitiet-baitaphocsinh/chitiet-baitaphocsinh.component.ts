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
  currentUserName = 'Nguyễn Văn A';
  @ViewChild('quillEditor') quillEditor!: QuillEditorComponent;

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

 
  submitAnswer(): void {
    const plainText = this.quillEditor.quillEditor.getText().trim();
  
    if (!plainText) {
      this.toastr.warning('Vui lòng nhập nội dung trước khi gửi.');
      return;
    }
  
    const formData = new FormData();
    formData.append('baiTapId', this.baiTapId);
    formData.append('noiDung', plainText); // Gửi văn bản thuần, không gửi HTML
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
            noiDung: plainText,
            file: newTraLoi.urlFile
              ? {
                  ten: this.getFileName(newTraLoi.urlFile),
                  kichThuoc: '---',
                }
              : null,
          });
  
          // Reset
          this.quillEditor.quillEditor.setText('');
          this.newAnswer = { noiDung: '', file: null };
          this.cdr.detectChanges();
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
            tenHocSinh: item.hocSinhTen,
            thoiGianNop: new Date(item.thoiGian).toLocaleTimeString([], { hour: '2-digit', minute: '2-digit' }),
            noiDung: item.noiDung,
            file: item.urlFile
              ? {
                  ten: this.getFileName(item.urlFile),
                  url: item.urlFile
                }
              : null
          }));
        }
      },
      error: (err) => {
        console.error(' Lỗi lấy danh sách trả lời:', err);
      }
    });
  }
  
  
  
  
  cancelAnswer(): void {
    this.newAnswer = { noiDung: '', file: null };
  }
  
  
}

