export enum UserRole {
    "Head of Department" = "Trưởng phòng",
    "Staff" = "Nhân viên",
}

export const UserStatus = {
    Pending: 1,
    Active: 2,
    Stopped: 3
};

export const UserStatusLabels = {
    [UserStatus.Pending]: 'Đang chờ',
    [UserStatus.Active]: 'Hoạt động',
    [UserStatus.Stopped]: 'Ngừng hoạt động'
};

export const UpdateUserStatusLabels = {
    [UserStatus.Active]: 'Hoạt động',
    [UserStatus.Stopped]: 'Ngừng hoạt động'
};
