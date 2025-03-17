import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { TableModule } from 'primeng/table';
import { CardModule } from 'primeng/card';
import { ActivatedRoute } from '@angular/router';
import { CurriculumApiService } from '../../../apis/curriculumAPIs/curriculum-api.service';

@Component({
  selector: 'app-plo-mapping',
  standalone: true,
  imports: [CommonModule, TableModule, CardModule],
  templateUrl: './plo.component.html'
})
export class PloComponent implements OnInit {
  curriculumId: string = '';
  programLearningOutcomes: any[] = [];
  uniqueSubjects: any[] = [];

  constructor(private route: ActivatedRoute, private curriculumService: CurriculumApiService) { }

  ngOnInit() {
    this.curriculumId = this.route.snapshot.paramMap.get('id') || '';
    this.getDetail();
  }

  getDetail() {
    const id = this.curriculumId;
    if (id) {
      this.curriculumService.getPLO(id).subscribe({
        next: (data) => {
          this.programLearningOutcomes = data;
          this.extractUniqueSubjects();
        },
        error: (err) => {
          console.error('Error fetching curriculum detail:', err);
        }
      });
    }
  }

  extractUniqueSubjects() {
    const subjectsSet = new Set<string>();
    this.programLearningOutcomes.forEach((plo) => {
      plo.subjects.forEach((subject: any) => subjectsSet.add(subject.subjectCode));
    });
    this.uniqueSubjects = Array.from(subjectsSet);
  }

  // Kiểm tra xem subject có trong PLO không
  isSubjectInPLO(plo: any, subjectCode: string): boolean {
    return plo.subjects.some((subject: any) => subject.subjectCode === subjectCode);
  }
}
