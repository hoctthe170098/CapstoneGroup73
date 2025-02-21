import { ComponentFixture, TestBed } from '@angular/core/testing';

import { DeleteloainhansuComponent } from './deleteloainhansu.component';

describe('DeleteloainhansuComponent', () => {
  let component: DeleteloainhansuComponent;
  let fixture: ComponentFixture<DeleteloainhansuComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ DeleteloainhansuComponent ]
    })
    .compileComponents();
  });

  beforeEach(() => {
    fixture = TestBed.createComponent(DeleteloainhansuComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
