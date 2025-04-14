import { ComponentFixture, TestBed } from '@angular/core/testing';

import { SyllabusHistoryComponent } from './syllabus-history.component';

describe('SyllabusHistoryComponent', () => {
  let component: SyllabusHistoryComponent;
  let fixture: ComponentFixture<SyllabusHistoryComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [SyllabusHistoryComponent]
    })
    .compileComponents();

    fixture = TestBed.createComponent(SyllabusHistoryComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
