export interface CreateChuongTrinh {
  id: number;
  tieuDe: string;
  moTa: string;
  noiDungBaiHocs?: CreateNoiDungBaiHoc[];
}

export interface CreateNoiDungBaiHoc {
  tieuDe: string;
  mota: string;
  soThuTu: number;
  taiLieuHocTaps?: CreateTaiLieuHocTap[];
}

export interface CreateTaiLieuHocTap {
  urlType: string;
  file?: File;
}

import { Injectable } from '@angular/core';
import { BehaviorSubject, Observable, of } from 'rxjs';

@Injectable({
providedIn: 'root'
})
export class ChuongtrinhService {

private programsSource = new BehaviorSubject<CreateChuongTrinh[]>(this.getStoredPrograms());
programs$ = this.programsSource.asObservable();

constructor() {}

private getStoredPrograms(): CreateChuongTrinh[] {
  const storedData = localStorage.getItem('programs');
  if (storedData) {
    return JSON.parse(storedData) as CreateChuongTrinh[];
  }
  
  const defaultPrograms: CreateChuongTrinh[] = [
    {
      id: 1,
      tieuDe: 'Chương trình học 1',
      moTa: 'Mô tả chương trình học 1',
      noiDungBaiHocs: [
        {
          
          tieuDe: 'Bài 1',
          mota: '',
          soThuTu: 1,
          taiLieuHocTaps: [
            { urlType: 'pdf', file: new File([], 'sample-lesson-1.pdf') }
          ]
        },
        {
          tieuDe: 'Bài 2',
          mota: '',
          soThuTu: 2,
          taiLieuHocTaps: [
            { urlType: 'pdf', file: new File([], 'sample-lesson-2.pdf') }
          ]
        },
        {
          tieuDe: 'Bài 3',
          mota: '',
          soThuTu: 3,
          taiLieuHocTaps: [
            { urlType: 'pdf', file: new File([], 'sample-lesson-3.pdf') }
          ]
        }
        
      ]
    }
  ];
  
  localStorage.setItem('programs', JSON.stringify(defaultPrograms));
  return defaultPrograms;
}

private updateLocalStorage(programs: CreateChuongTrinh[]) {
  localStorage.setItem('programs', JSON.stringify(programs));
}

getPrograms(): CreateChuongTrinh[] {
  const storedData = localStorage.getItem('programs');
  return storedData ? JSON.parse(storedData) : [];
}


getProgram(id: number): CreateChuongTrinh | null {
  const programs = this.programsSource.value;
  return programs.find(program => program.id === id) || null; 
}


updateProgram(id: number, updatedProgram: CreateChuongTrinh): void {
  let programs = this.getPrograms();
  const index = programs.findIndex(p => p.id === id);
  
  if (index !== -1) {
    programs[index] = { ...updatedProgram };
    this.updateLocalStorage(programs);
    this.programsSource.next(programs);  
  }
}


addProgram(newProgram: CreateChuongTrinh): void {
  const programs = this.programsSource.value.slice();
  const newId = programs.reduce((max, p) => (p.id > max ? p.id : max), 0) + 1;
  programs.push({ ...newProgram, id: newId }); // Gán id mới
  this.programsSource.next(programs);
  this.updateLocalStorage(programs);
}


deleteProgram(index: number): void {
  const programs = this.programsSource.value.slice();
  programs.splice(index, 1);
  this.programsSource.next(programs);
  this.updateLocalStorage(programs);
}

uploadFile(file: File): Observable<string> {
  const fileName = encodeURIComponent(file.name);
  const fakeServerUrl = `https://example.com/uploads/${fileName}`;
  return of(fakeServerUrl);
}
}
