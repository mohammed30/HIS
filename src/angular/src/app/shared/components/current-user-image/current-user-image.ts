import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ProfileService } from '@abp/ng.account.core/proxy';
import { ConfigStateService } from '@abp/ng.core';

@Component({
  selector: 'app-current-user-image',
  standalone: true,
  imports: [CommonModule],
  template: `
    <img
      *ngIf="profilePictureUrl; else defaultAvatar"
      [src]="profilePictureUrl"
      class="rounded-circle lpx-user-avatar"
      style="width: 35px; height: 35px; object-fit: cover;"
      alt="User Profile"
    />
    <ng-template #defaultAvatar>
      <img
        *ngIf="userName"
        [src]="'https://ui-avatars.com/api/?name=' + userName + '&background=random&rounded=true&size=35'"
        class="rounded-circle lpx-user-avatar"
        style="width: 35px; height: 35px; object-fit: cover;"
        alt="User Profile"
      />
    </ng-template>
  `,
  styles: []
})
export class CurrentUserImageComponent implements OnInit {
  profilePictureUrl: string | null = null;
  userName: string = '';

  constructor(
    private profileService: ProfileService,
    private configState: ConfigStateService
  ) {}

  ngOnInit(): void {
    const currentUser = this.configState.getOne('currentUser');
    this.userName = currentUser?.userName || 'U';

    this.profileService.get().subscribe(profile => {
      if (profile.extraProperties && profile.extraProperties['ProfilePictureUrl']) {
        this.profilePictureUrl = profile.extraProperties['ProfilePictureUrl'] as string;
      }
    });
  }
}
