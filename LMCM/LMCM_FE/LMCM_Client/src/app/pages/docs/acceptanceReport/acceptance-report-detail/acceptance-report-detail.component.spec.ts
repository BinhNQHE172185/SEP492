import { ComponentFixture, TestBed } from '@angular/core/testing';

import { AcceptanceReportDetailComponent } from './acceptance-report-detail.component';

describe('AcceptanceReportDetailComponent', () => {
  let component: AcceptanceReportDetailComponent;
  let fixture: ComponentFixture<AcceptanceReportDetailComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [AcceptanceReportDetailComponent]
    })
    .compileComponents();

    fixture = TestBed.createComponent(AcceptanceReportDetailComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
