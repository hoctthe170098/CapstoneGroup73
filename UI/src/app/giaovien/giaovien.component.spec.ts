import { ComponentFixture, TestBed } from '@angular/core/testing';

import { GiaovienComponent } from './giaovien.component';

describe('GiaovienComponent', () => {
  let component: GiaovienComponent;
  let fixture: ComponentFixture<GiaovienComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ GiaovienComponent ]
    })
    .compileComponents();
  });

  beforeEach(() => {
    fixture = TestBed.createComponent(GiaovienComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
