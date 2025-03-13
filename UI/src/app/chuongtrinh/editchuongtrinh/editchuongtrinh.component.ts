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
    title: '',
    description: '',
    lessons: []
  };

  constructor(
    private route: ActivatedRoute,
    private router: Router,
    private chuongtrinhService: ChuongtrinhService
  ) {}

  ngOnInit(): void {
    const id = this.route.snapshot.paramMap.get('id');
    this.programId = id ? parseInt(id, 10) : null;

    if (this.programId !== null) {
      const loadedProgram = this.chuongtrinhService.getProgram(this.programId);
      // Đảm bảo mỗi lesson có mảng files và thuộc tính expanded
      this.program = {
        ...loadedProgram,
        lessons: loadedProgram.lessons.map((lesson: any) => {
          // Nếu lesson cũ chỉ có fileUrl, chuyển sang mảng files
          if (!lesson.files) {
            lesson.files = lesson.fileUrl
              ? [{ name: this.extractFileName(lesson.fileUrl), fileUrl: lesson.fileUrl }]
              : [];
          }
          // Gán expanded = false nếu chưa có
          if (lesson.expanded === undefined) {
            lesson.expanded = false;
          }
          return lesson;
        })
      };
    }
  }

  // Tách tên file từ URL
  extractFileName(fileUrl: string): string {
    return fileUrl.substring(fileUrl.lastIndexOf('/') + 1);
  }

  // Thêm bài học mới
  addLesson() {
    this.program.lessons.push({
      title: '',
      description: '',
      files: [],
      expanded: false
    });
  }

  // Toggle đóng/mở bài học
  toggleLesson(index: number) {
    this.program.lessons[index].expanded = !this.program.lessons[index].expanded;
  }

  // Xóa hẳn một bài học
  removeLesson(index: number) {
    this.program.lessons.splice(index, 1);
  }

  // (Tuỳ chọn) Xóa nội dung bài học (nếu bạn muốn xóa title, description, files mà không xóa bài học)
  clearLesson(index: number) {
    this.program.lessons[index].title = '';
    this.program.lessons[index].description = '';
    this.program.lessons[index].files = [];
  }

  // Kéo thả file
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

  // Chọn file
  onFileChange(event: Event, lessonIndex: number) {
    const files = (event.target as HTMLInputElement).files;
    if (files) {
      for (let i = 0; i < files.length; i++) {
        this.uploadFile(files[i], lessonIndex);
      }
    }
  }

  // Upload file (giả lập, cho phép nhiều loại file)
  uploadFile(file: File, lessonIndex: number) {
    // Ví dụ cho phép PDF, Word, MP4, ZIP...
    const allowedTypes = [
      'application/pdf',
      'application/msword',
      'application/vnd.openxmlformats-officedocument.wordprocessingml.document',
      'video/mp4',
      'application/zip'
    ];
    if (!allowedTypes.includes(file.type)) {
      alert('Chỉ chấp nhận file PDF, Word, MP4, hoặc ZIP!');
      return;
    }

    // Gọi service upload file (giả lập)
    this.chuongtrinhService.uploadFile(file).subscribe(
      fileUrl => {
        const fileName = file.name;
        if (!this.program.lessons[lessonIndex].files) {
          this.program.lessons[lessonIndex].files = [];
        }
        this.program.lessons[lessonIndex].files.push({ name: fileName, fileUrl });
        console.log(`File uploaded for lesson ${lessonIndex + 1}: ${fileUrl}`);
      },
      error => {
        console.error('Lỗi upload file:', error);
        alert('Upload file thất bại!');
      }
    );
  }

  // Xóa 1 file trong bài học
  removeFile(lessonIndex: number, fileIndex: number) {
    this.program.lessons[lessonIndex].files.splice(fileIndex, 1);
  }

  // Mở file trên tab mới
  openFile(fileUrl: string) {
    if (fileUrl) {
      window.open(fileUrl, '_blank');
    } else {
      alert('Không có tài liệu để mở!');
    }
  }

  // Lưu chương trình
  saveProgram() {
    console.log('Program to save:', JSON.stringify(this.program, null, 2));
    if (this.programId !== null) {
      this.chuongtrinhService.updateProgram(this.programId, this.program);
    }
    alert('Lưu thành công!');
    this.router.navigate(['/chuongtrinh']);
  }

  // Hủy chỉnh sửa
  cancelEdit() {
    this.router.navigate(['/chuongtrinh']);
  }
}
