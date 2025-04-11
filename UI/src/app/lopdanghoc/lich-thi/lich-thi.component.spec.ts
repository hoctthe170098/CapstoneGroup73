import { ComponentFixture, TestBed } from '@angular/core/testing';

import { LichThiComponent } from './lich-thi.component';

describe('LichThiComponent', () => {
  let component: LichThiComponent;
  let fixture: ComponentFixture<LichThiComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ LichThiComponent ]
    })
    .compileComponents();
  });

  beforeEach(() => {
    fixture = TestBed.createComponent(LichThiComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
