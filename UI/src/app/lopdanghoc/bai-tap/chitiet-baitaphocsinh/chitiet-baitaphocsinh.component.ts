import { Component, OnInit, OnDestroy, ChangeDetectorRef } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import { saveAs } from 'file-saver';
import { LopdanghocService } from 'app/lopdanghoc/shared/lopdanghoc.service';

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
  answers: any[] = [
    {
      tenHocSinh: 'Nguyễn Văn A',
      thoiGianNop: '13:00',
      noiDung: 'Lorem ipsum dolor sit amet, consectetur adipiscing elit...',
      file: {
        ten: 'File Title.pdf',
        kichThuoc: '313 KB'
      }
    }
  ];

  constructor(
    private route: ActivatedRoute,
    private lopdanghocService: LopdanghocService,
    private cdr: ChangeDetectorRef
  ) {}

  ngOnInit(): void {
    this.route.paramMap.subscribe(params => {
      this.baiTapId = params.get('baiTapId') || '';
      if (this.baiTapId) {
        this.loadBaiTapDetail();
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
      this.updateCountdownDisplay(remaining);
      if (remaining <= 0) clearInterval(this.intervalId);
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

  downloadFile(): void {
    if (!this.baiTapDetail?.urlFile) return;
    const filePath = this.baiTapDetail.urlFile;
    this.lopdanghocService.downloadBaiTapFile(filePath).subscribe({
      next: (res) => {
        if (res?.data?.fileContents) {
          const byteArray = new Uint8Array(atob(res.data.fileContents).split('').map(char => char.charCodeAt(0)));
          const blob = new Blob([byteArray], { type: res.data.contentType });
          const fileName = res.data.fileDownloadName || 'baitap.pdf';
          saveAs(blob, fileName);
        }
      },
      error: (err) => {
        console.error('Lỗi khi tải file:', err);
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
    if (!this.newAnswer.noiDung.trim()) {
      alert('Nội dung không được để trống.');
      return;
    }
  
    console.log('📝 Submitting answer:', this.newAnswer);
    // TODO: Gửi về API ở đây
  
    // Giả lập thêm vào danh sách (sau khi gửi thành công)
    this.answers.unshift({
      tenHocSinh: this.currentUserName,
      thoiGianNop: new Date().toLocaleTimeString([], { hour: '2-digit', minute: '2-digit' }),
      noiDung: this.newAnswer.noiDung,
      file: this.newAnswer.file
        ? {
            ten: this.newAnswer.file.name,
            kichThuoc: `${Math.round(this.newAnswer.file.size / 1024)} KB`
          }
        : null
    });
  
    // Reset form
    this.newAnswer = { noiDung: '', file: null };
  }
  
  cancelAnswer(): void {
    this.newAnswer = { noiDung: '', file: null };
  }
  
  
}

