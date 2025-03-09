import { ComponentFixture, TestBed } from '@angular/core/testing';

import { EditchuongtrinhComponent } from './editchuongtrinh.component';

describe('EditchuongtrinhComponent', () => {
  let component: EditchuongtrinhComponent;
  let fixture: ComponentFixture<EditchuongtrinhComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ EditchuongtrinhComponent ]
    })
    .compileComponents();
  });

  beforeEach(() => {
    fixture = TestBed.createComponent(EditchuongtrinhComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
