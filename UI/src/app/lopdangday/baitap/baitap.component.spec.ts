import { ComponentFixture, TestBed } from '@angular/core/testing';

import { BaitapComponent } from './baitap.component';

describe('BaitapComponent', () => {
  let component: BaitapComponent;
  let fixture: ComponentFixture<BaitapComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ BaitapComponent ]
    })
    .compileComponents();
  });

  beforeEach(() => {
    fixture = TestBed.createComponent(BaitapComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
