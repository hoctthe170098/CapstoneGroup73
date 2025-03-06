import { Component } from '@angular/core';

@Component({
  selector: 'app-chuongtrinh',
  templateUrl: './chuongtrinh.component.html',
  styleUrls: ['./chuongtrinh.component.scss']
})
export class ChuongtrinhComponent { 
  programs = [
    {
      title: 'Chương trình học 1',
      description: 'Some quick example text to build on the card title and make up the bulk of the card\'s content.',
      expanded: false,
      lessons: [
        { title: 'Nội dung bài học 1', content: 'With supporting text below as a natural lead-in to additional content.' },
        { title: 'Nội dung bài học 2', content: 'With supporting text below as a natural lead-in to additional content.' }
      ]
    },
    {
      title: 'Chương trình học 2',
      description: 'Some quick example text to build on the card title and make up the bulk of the card\'s content.',
      expanded: false,
      lessons: [
        { title: 'Nội dung bài học', content: 'With supporting text below as a natural lead-in to additional content.' }
      ]
    },
    {
      title: 'Chương trình học 3',
      description: 'Some quick example text to build on the card title and make up the bulk of the card\'s content.',
      expanded: false,
      lessons: [
        { title: 'Nội dung bài học', content: 'With supporting text below as a natural lead-in to additional content.' }
      ]
    }
  ];

  toggleContent(index: number, event: Event) {
    event.preventDefault(); // Ngăn chặn load lại trang
    this.programs[index].expanded = !this.programs[index].expanded;
  }
}