import { ComponentFixture, TestBed } from '@angular/core/testing';

import { ChuongtrinhhocComponent } from './chuongtrinhhoc.component';

describe('ChuongtrinhhocComponent', () => {
  let component: ChuongtrinhhocComponent;
  let fixture: ComponentFixture<ChuongtrinhhocComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ ChuongtrinhhocComponent ]
    })
    .compileComponents();
  });

  beforeEach(() => {
    fixture = TestBed.createComponent(ChuongtrinhhocComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
