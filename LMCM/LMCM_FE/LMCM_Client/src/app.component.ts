import { Component, OnInit } from '@angular/core';
import { Router, RouterModule } from '@angular/router';
import { CookieService } from 'ngx-cookie-service';

@Component({
    selector: 'app-root',
    standalone: true,
    imports: [RouterModule],
    template: `<router-outlet></router-outlet>`
})
export class AppComponent implements OnInit {

    constructor(
        private router: Router,
        private cookieService: CookieService
    ) { }

    ngOnInit(): void {
        const token = this.cookieService.get('AuthToken');

        if (!token && this.router.url !== '/auth/login') {
            this.router.navigate(['/auth/login']);
        }
    }
}
