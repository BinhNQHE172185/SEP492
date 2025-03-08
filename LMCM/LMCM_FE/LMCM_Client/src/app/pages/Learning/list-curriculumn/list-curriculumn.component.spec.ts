import { ComponentFixture, TestBed } from '@angular/core/testing';
import { ListCurriculumnComponent } from './list-curriculumn.component';
import { FormsModule } from '@angular/forms';

describe('ListCurriculumnComponent', () => {
  let component: ListCurriculumnComponent;
  let fixture: ComponentFixture<ListCurriculumnComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ListCurriculumnComponent],
      imports: [FormsModule] // Đảm bảo có FormsModule để tránh lỗi với [(ngModel)]
    }).compileComponents();

    fixture = TestBed.createComponent(ListCurriculumnComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });

  it('should filter programs correctly', () => {
    component.searchText = 'Data';
    expect(component.filteredPrograms().length).toBe(1);
    expect(component.filteredPrograms()[0].name).toBe('Data Science');
  });

  it('should delete a program', () => {
    component.deleteItem('CUR001');
    expect(component.programs.length).toBe(3);
    expect(component.programs.find(p => p.code === 'CUR001')).toBeUndefined();
  });

  it('should navigate pages correctly', () => {
    component.setPage(2);
    expect(component.currentPage).toBe(2);

    component.prevPage();
    expect(component.currentPage).toBe(1);

    component.nextPage();
    expect(component.currentPage).toBe(2);
  });
});
