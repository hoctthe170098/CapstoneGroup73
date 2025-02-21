import { ComponentFixture, TestBed } from '@angular/core/testing';

import { UpdatechuongtrinhComponent } from './updatechuongtrinh.component';

describe('UpdatechuongtrinhComponent', () => {
  let component: UpdatechuongtrinhComponent;
  let fixture: ComponentFixture<UpdatechuongtrinhComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ UpdatechuongtrinhComponent ]
    })
    .compileComponents();
  });

  beforeEach(() => {
    fixture = TestBed.createComponent(UpdatechuongtrinhComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
