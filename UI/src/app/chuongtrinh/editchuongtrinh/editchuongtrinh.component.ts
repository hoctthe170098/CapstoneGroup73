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
      const loadedProgram = this.chuongtrinhService.getProgram(this.programId);
      
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
      this.chuongtrinhService.updateProgram(this.programId, this.program);
      alert('Lưu thành công!');
      this.router.navigate(['/chuongtrinh']).then(() => {
        window.location.reload(); // 🔥 Đảm bảo trang được load lại hoàn toàn
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