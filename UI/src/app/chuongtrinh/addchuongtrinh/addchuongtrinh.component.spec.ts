import { ComponentFixture, TestBed } from '@angular/core/testing';

import { AddchuongtrinhComponent } from './addchuongtrinh.component';

describe('AddchuongtrinhComponent', () => {
  let component: AddchuongtrinhComponent;
  let fixture: ComponentFixture<AddchuongtrinhComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ AddchuongtrinhComponent ]
    })
    .compileComponents();
  });

  beforeEach(() => {
    fixture = TestBed.createComponent(AddchuongtrinhComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
