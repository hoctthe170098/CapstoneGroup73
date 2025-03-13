import { ComponentFixture, TestBed } from '@angular/core/testing';

import { ImportHocsinhComponent } from './import-hocsinh.component';

describe('ImportHocsinhComponent', () => {
  let component: ImportHocsinhComponent;
  let fixture: ComponentFixture<ImportHocsinhComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ ImportHocsinhComponent ]
    })
    .compileComponents();
  });

  beforeEach(() => {
    fixture = TestBed.createComponent(ImportHocsinhComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
