import { ComponentFixture, TestBed } from '@angular/core/testing';

import { CreatechuongtrinhComponent } from './createchuongtrinh.component';

describe('CreatechuongtrinhComponent', () => {
  let component: CreatechuongtrinhComponent;
  let fixture: ComponentFixture<CreatechuongtrinhComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ CreatechuongtrinhComponent ]
    })
    .compileComponents();
  });

  beforeEach(() => {
    fixture = TestBed.createComponent(CreatechuongtrinhComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
