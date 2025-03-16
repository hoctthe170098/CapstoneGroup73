import { Component, OnInit } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { ChuongtrinhService } from '../shared/chuongtrinh.service';

@Component({
  selector: 'app-editchuongtrinh',
  templateUrl: './editchuongtrinh.component.html',
  styleUrls: ['./editchuongtrinh.component.scss']
})
export class EditchuongtrinhComponent implements OnInit {

  programId: number | null = null;
  program: any = {
    id: 0,
    tieuDe: '',
    moTa: '',
    noiDungBaiHocs: []
  };

  constructor(
    private route: ActivatedRoute,
    private router: Router,
    private chuongtrinhService: ChuongtrinhService
  ) {}

  ngOnInit(): void {
    this.programId = Number(this.route.snapshot.paramMap.get('id'));
  
    if (this.programId !== null && !isNaN(this.programId)) {
      this.chuongtrinhService.getPrograms(this.programId).subscribe({
        next: (loadedProgram) => {
          if (loadedProgram) {
            
            this.program = {
              ...loadedProgram,
              noiDungBaiHocs: loadedProgram.noiDungBaiHocs?.map((lesson: any, index: number) => ({
                ...lesson,
                soThuTu: lesson.soThuTu ?? index + 1,
                taiLieuHocTaps: lesson.taiLieuHocTaps ?? [],
                expanded: lesson.expanded ?? false
              })) ?? []
            };
          } else {
            console.error("Không tìm thấy chương trình học với ID:", this.programId);
          }
        },
        error: (err) => {
          console.error("Lỗi khi tải chương trình học:", err);
        }
      });
    }
  }

  addLesson() {
    this.program.noiDungBaiHocs.push({
      tieuDe: '',
      mota: '',
      soThuTu: this.program.noiDungBaiHocs.length + 1,
      taiLieuHocTaps: [],
      expanded: false
    });
  }

  toggleLesson(index: number) {
    this.program.noiDungBaiHocs[index].expanded = !this.program.noiDungBaiHocs[index].expanded;
  }

  removeLesson(index: number) {
    this.program.noiDungBaiHocs.splice(index, 1);
    this.program.noiDungBaiHocs.forEach((lesson, i) => {
      lesson.soThuTu = i + 1;
    });
  }

  saveProgram() {
    if (this.programId !== null) {
      // ✅ Định dạng dữ liệu payload gửi lên API
      const payload: any = {
        chuongTrinhDto: {
          id: this.programId,
          tieuDe: this.program.tieuDe,
          moTa: this.program.moTa,
          trangThai: this.program.trangThai ?? "Đang cập nhật",
          noiDungBaiHocs: this.program.noiDungBaiHocs.map((lesson: any) => ({
            id: lesson.id ?? "0", // Nếu không có ID, mặc định là "0"
            tieuDe: lesson.tieuDe,
            mota: lesson.mota,
            soThuTu: lesson.soThuTu,
            taiLieuHocTaps: lesson.taiLieuHocTaps.map((file: any) => ({
              id: file.id ?? "0",
              urlType: file.urlType,
              file: file.fileUrl ?? "" // ✅ Đảm bảo `fileUrl` tồn tại
            }))
          }))
        }
      };
  
      // ✅ Gọi API cập nhật chương trình học
      this.chuongtrinhService.updateProgram(payload).subscribe({
        next: (response) => {
          alert('Chỉnh sửa chương trình học thành công!');
          this.router.navigate(['/chuongtrinh']).then(() => {
            window.location.reload(); // 🔥 Load lại trang
          });
        },
        error: (error) => {
          console.error('❌ Lỗi khi cập nhật chương trình:', error);
          alert('Lỗi khi cập nhật chương trình học!');
        }
      });
    }
  }
  
  

  cancelEdit() {
    this.router.navigate(['/chuongtrinh']);
  }

  onDragOver(event: DragEvent) {
    event.preventDefault();
  }

  onDrop(event: DragEvent, lessonIndex: number) {
    event.preventDefault();
    if (event.dataTransfer?.files.length) {
      for (let i = 0; i < event.dataTransfer.files.length; i++) {
        this.uploadFile(event.dataTransfer.files[i], lessonIndex);
      }
    }
  }

  onFileChange(event: Event, lessonIndex: number) {
    const files = (event.target as HTMLInputElement).files;
    if (files) {
      for (let i = 0; i < files.length; i++) {
        this.uploadFile(files[i], lessonIndex);
      }
    }
  }
  removeFile(lessonIndex: number, fileIndex: number) {
    this.program.noiDungBaiHocs[lessonIndex].taiLieuHocTaps.splice(fileIndex, 1);
  }

  uploadFile(file: File, lessonIndex: number) {
    const allowedTypes = ['application/pdf','application/word','video/mp4'];
    if (!allowedTypes.includes(file.type)) {
      alert('Chỉ chấp nhận file PDF, Word, MP4, hoặc ZIP!');
      return;
    }
    this.chuongtrinhService.uploadFile(file).subscribe(
      fileUrl => {
        const fileName = file.name;
        if (!this.program.noiDungBaiHocs[lessonIndex].taiLieuHocTaps) {
          this.program.noiDungBaiHocs[lessonIndex].taiLieuHocTaps = [];
        }
        this.program.noiDungBaiHocs[lessonIndex].taiLieuHocTaps.push({ urlType: file.type, fileUrl });
      },
      error => {
        console.error('Lỗi upload file:', error);
        alert('Upload file thất bại!');
      }
    );
  }
}