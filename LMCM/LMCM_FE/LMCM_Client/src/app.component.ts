import { Component, OnInit } from '@angular/core';
import { Router, RouterModule } from '@angular/router';

@Component({
    selector: 'app-root',
    standalone: true,
    imports: [RouterModule],
    template: `<router-outlet></router-outlet>`
})
export class AppComponent implements OnInit {

    constructor(private router: Router) { }

    ngOnInit(): void {
        const token = localStorage.getItem('token');

        if (!token && this.router.url !== '/auth/login') {
            this.router.navigate(['/auth/login']);
        }
    }
}
