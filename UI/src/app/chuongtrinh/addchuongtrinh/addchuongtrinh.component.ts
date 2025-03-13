import { Component } from '@angular/core';
import { Router } from '@angular/router';
import { ChuongtrinhService } from '../shared/chuongtrinh.service';

@Component({
  selector: 'app-addchuongtrinh',
  templateUrl: './addchuongtrinh.component.html',
  styleUrls: ['./addchuongtrinh.component.scss']
})
export class AddchuongtrinhComponent {
  program: any = {
    title: '',
    description: '',
    lessons: [
      {
        title: '',
        description: '',
        files: [] as any[],
        expanded: false
      }
    ]
  };

  constructor(
    private router: Router,
    private chuongtrinhService: ChuongtrinhService
  ) { }

  addLesson() {
    this.program.lessons.push({
      title: '',
      description: '',
      files: [],
      expanded: false
    });
  }

  removeLesson(index: number) {
    this.program.lessons.splice(index, 1);
  }

  toggleLesson(index: number) {
    this.program.lessons[index].expanded = !this.program.lessons[index].expanded;
  }
  

  onFileChange(event: Event, lessonIndex: number) {
    const files = (event.target as HTMLInputElement).files;
    if (files) {
      for (let i = 0; i < files.length; i++) {
        this.uploadFile(files[i], lessonIndex);
      }
    }
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

  uploadFile(file: File, lessonIndex: number) {
    const allowedTypes = ['application/pdf', 'image/png', 'image/jpeg', 'image/svg+xml', 'application/zip'];
    if (!allowedTypes.includes(file.type)) {
      alert('Chỉ chấp nhận file .pdf, .jpg, .png, .svg, .zip!');
      return;
    }
    // Giả lập fileUrl, bạn có thể gọi service.uploadFile(file) nếu muốn lấy URL thật
    const fileUrl = `assets/files/${file.name}`;
    this.program.lessons[lessonIndex].files.push({ name: file.name, fileUrl });
  }

  removeFile(lessonIndex: number, fileIndex: number) {
    this.program.lessons[lessonIndex].files.splice(fileIndex, 1);
  }

  saveProgram() {
    console.log('Chương trình:', JSON.stringify(this.program, null, 2));
    this.chuongtrinhService.addProgram(this.program);
    alert('Thêm chương trình thành công!');
    this.router.navigate(['/chuongtrinh']);
  }

  cancel() {
    this.router.navigate(['/chuongtrinh']);
  }
}
