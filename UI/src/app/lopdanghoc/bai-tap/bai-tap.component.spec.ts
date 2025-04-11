import { ComponentFixture, TestBed } from '@angular/core/testing';

import { BaiTapComponent } from './bai-tap.component';

describe('BaiTapComponent', () => {
  let component: BaiTapComponent;
  let fixture: ComponentFixture<BaiTapComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ BaiTapComponent ]
    })
    .compileComponents();
  });

  beforeEach(() => {
    fixture = TestBed.createComponent(BaiTapComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
