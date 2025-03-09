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
    lessons: [{ title: '', description: '', fileUrl: '', file: null }]
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
      this.program = {
        ...loadedProgram,
        lessons: loadedProgram.lessons.map((lesson: any) => ({
          ...lesson,
          file: null // Không cần giữ file object trong program
        }))
      };
    }
  }

  addLesson() {
    this.program.lessons.push({ title: '', description: '', fileUrl: '', file: null });
  }

  removeLesson(index: number) {
    this.program.lessons.splice(index, 1);
  }

  onFileChange(event: Event, index: number) {
    const file = (event.target as HTMLInputElement).files?.[0];
    if (file) {
      if (file.type !== 'application/pdf') {
        alert('Chỉ chấp nhận file PDF!');
        return;
      }
      this.uploadFile(file, index);
    }
  }

  uploadFile(file: File, index: number) {
    this.chuongtrinhService.uploadFile(file).subscribe(fileUrl => {
      this.program.lessons[index].fileUrl = fileUrl; // Gán URL thật từ "server"
      console.log(`File uploaded for lesson ${index + 1}: ${fileUrl}`);
    }, error => {
      console.error('Lỗi upload file:', error);
      alert('Upload file thất bại!');
    });
  }

  saveProgram() {
    console.log('Program to save:', JSON.stringify(this.program, null, 2));
    if (this.programId !== null) {
      this.chuongtrinhService.updateProgram(this.programId, this.program);
    }
    alert('Lưu thành công!');
    this.router.navigate(['/chuongtrinh']);
  }

  cancelEdit() {
    this.router.navigate(['/chuongtrinh']);
  }

  openFile(fileUrl: string) {
    if (fileUrl) {
      window.open(fileUrl, '_blank');
    } else {
      alert('Không có tài liệu để mở!');
    }
  }
}
