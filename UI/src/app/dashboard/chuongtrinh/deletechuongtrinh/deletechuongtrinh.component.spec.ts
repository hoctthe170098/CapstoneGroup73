import { ComponentFixture, TestBed } from '@angular/core/testing';

import { DeletechuongtrinhComponent } from './deletechuongtrinh.component';

describe('DeletechuongtrinhComponent', () => {
  let component: DeletechuongtrinhComponent;
  let fixture: ComponentFixture<DeletechuongtrinhComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ DeletechuongtrinhComponent ]
    })
    .compileComponents();
  });

  beforeEach(() => {
    fixture = TestBed.createComponent(DeletechuongtrinhComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
