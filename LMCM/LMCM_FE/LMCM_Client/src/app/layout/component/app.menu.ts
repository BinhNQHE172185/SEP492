import { Component } from '@angular/core';
import { CommonModule } from '@angular/common';
import { Router, RouterModule } from '@angular/router';
import { MenuItem } from 'primeng/api';
import { AppMenuitem } from './app.menuitem';
import { CookieService } from 'ngx-cookie-service';
import { jwtDecode } from 'jwt-decode';

@Component({
    selector: 'app-menu',
    standalone: true,
    imports: [CommonModule, AppMenuitem, RouterModule],
    template: `<ul class="layout-menu">
        <ng-container *ngFor="let item of model; let i = index">
            <li app-menuitem *ngIf="!item.separator" [item]="item" [index]="i" [root]="true"></li>
            <li *ngIf="item.separator" class="menu-separator"></li>
        </ng-container>
    </ul> `
})
export class AppMenu {
    isHeadOfDepartment(): boolean {
        const token = this.cookieService.get('AuthToken');
        if (!token) return false;

        try {
            const decoded: any = jwtDecode(token);
            return decoded.role?.toLowerCase() === 'head of department';
        } catch {
            return false;
        }
    }

    model: MenuItem[] = [];
    constructor(
        private router: Router,
        private cookieService: CookieService
    ) { }

    ngOnInit() {
        this.model = [
            {
                label: 'Trang chủ',
                items: [{ label: 'Dashboard', icon: 'pi pi-fw pi-home', routerLink: ['/'] }]
            },
            {
                label: 'Quản lý học liệu',
                icon: 'pi pi-fw pi-briefcase',
                routerLink: ['/learning'],
                items: [
                    {
                        label: 'Quản lý môn học',
                        icon: 'pi pi-fw pi-book',
                        routerLink: ['learning/subject']
                    },
                    {
                        label: 'Quản lý chương trình',
                        icon: 'pi pi-fw pi-book',
                        routerLink: ['learning/curriculum'],
                        routerLinkActiveOptions: { exact: false },
                    },
                    {
                        label: 'Quản lý đề cương',
                        icon: 'pi pi-fw pi-book',
                        routerLink: ['learning/syllabus'],
                        routerLinkActiveOptions: { exact: false },
                    },
                    {
                        label: 'Lịch sử thay đổi',
                        icon: 'pi pi-fw pi-book',
                        routerLink: ['learning/history']
                    },
                ]
            },
            {
                label: 'Quản lý tài liệu',
                icon: 'pi pi-fw pi-briefcase',
                routerLink: ['/document'],
                items: [
                    {
                        label: 'Quản lý tờ trình',
                        icon: 'pi pi-fw pi-print',
                        routerLink: ['document/report']
                    },
                    {
                        label: 'Quản lý hợp đồng',
                        icon: 'pi pi-fw pi-print',
                        routerLink: ['document/contract']
                    },
                    {
                        label: 'Quản lý biên bản nghiệm thu',
                        icon: 'pi pi-fw pi-print',
                        routerLink: ['document/acceptance-report']
                    },
                    {
                        label: 'Quản lý chuyên gia',
                        icon: 'pi pi-fw pi-print',
                        routerLink: ['document/contractor']
                    },
                    {
                        label: 'Quản lý mẫu tài liệu',
                        icon: 'pi pi-fw pi-print',
                        routerLink: ['document/template']
                    },
                ]
            },
            ...(this.isHeadOfDepartment()
                ? [
                    {
                        label: 'Nhân sự',
                        icon: 'pi pi-fw pi-briefcase',
                        routerLink: ['/user'],
                        items: [
                            { label: 'Quản lý nhân sự', icon: 'pi pi-fw pi-id-card', routerLink: ['user/staff'] }
                        ]
                    },
                    {
                        label: 'Khác',
                        items: [
                            { label: 'Bảng tính giá trị', icon: 'pi pi-fw pi-dollar', routerLink: ['/others/contract-value'] },
                        ]
                    }
                ]
                : []
            ),
            {
                label: 'Người dùng',
                items: [
                    { label: 'Trang cá nhân', icon: 'pi pi-fw pi-user', routerLink: ['/user/profile'] },
                    {
                        label: 'Đăng xuất', icon: 'pi pi-sign-out',
                        command: () => this.logout() // Gọi hàm logout
                    },
                ]
            }
        ];
    }
    logout() {
        this.cookieService.delete('AuthToken', '/', '');
        this.router.navigate(['/auth/login']);
    }
}
