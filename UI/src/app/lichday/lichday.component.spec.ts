import { ComponentFixture, TestBed } from '@angular/core/testing';

import { LichDayComponent } from './lichday.component';

describe('LichdayComponent', () => {
  let component: LichDayComponent;
  let fixture: ComponentFixture<LichDayComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ LichDayComponent ]
    })
    .compileComponents();
  });

  beforeEach(() => {
    fixture = TestBed.createComponent(LichDayComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
