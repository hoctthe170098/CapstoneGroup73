import { ComponentFixture, TestBed } from '@angular/core/testing';

import { ImportGiaovienComponent } from './import-giaovien.component';

describe('ImportGiaovienComponent', () => {
  let component: ImportGiaovienComponent;
  let fixture: ComponentFixture<ImportGiaovienComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ ImportGiaovienComponent ]
    })
    .compileComponents();
  });

  beforeEach(() => {
    fixture = TestBed.createComponent(ImportGiaovienComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
