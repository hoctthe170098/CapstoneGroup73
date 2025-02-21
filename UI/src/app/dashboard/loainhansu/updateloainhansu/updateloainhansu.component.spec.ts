import { ComponentFixture, TestBed } from '@angular/core/testing';

import { UpdateloainhansuComponent } from './updateloainhansu.component';

describe('UpdateloainhansuComponent', () => {
  let component: UpdateloainhansuComponent;
  let fixture: ComponentFixture<UpdateloainhansuComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ UpdateloainhansuComponent ]
    })
    .compileComponents();
  });

  beforeEach(() => {
    fixture = TestBed.createComponent(UpdateloainhansuComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
