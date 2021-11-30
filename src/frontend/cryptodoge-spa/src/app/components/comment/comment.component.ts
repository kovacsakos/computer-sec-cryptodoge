import { ToastService } from 'src/app/services/toast.service';
import { UserService } from './../../services/user.service';
import { CaffCommentReturnDto } from './../../../../generated/client/model/caffCommentReturnDto';
import { Component, EventEmitter, Input, OnInit, Output, ViewChild } from '@angular/core';
import { ImagesService } from 'generated/client';
import { ConfirmationService } from 'primeng/api';

@Component({
  selector: 'app-comment',
  templateUrl: './comment.component.html',
  styleUrls: ['./comment.component.scss']
})
export class CommentComponent implements OnInit {

  @Input('comment') comment: CaffCommentReturnDto
  @Output() commentShouldBeDeleted = new EventEmitter<string>();

  updatedComment: string;
  @ViewChild('inplace') inPlace;

  constructor(private toaster: ToastService, private userService: UserService, private imagesService: ImagesService, private confirmationService: ConfirmationService) {
    
  }

  ngOnInit() {
    this.updatedComment = this.comment.comment;
  }

  updateComment() {
    this.imagesService.imagesUpdateComment(this.comment.id, {comment: this.updatedComment}).subscribe(() => {
      this.comment.comment = this.updatedComment;
      this.inPlace.deactivate();
      this.toaster.success("Comment updated");
    })
  }

  editComment() {
    this.inPlace.activate();
  }

 

  deleteComment(event: Event) {
    this.confirmationService.confirm({
        target: event.target,
        message: 'Are you sure that you want to delete this comment?',
        icon: 'pi pi-exclamation-triangle',
        accept: () => {
            //confirm action
            this.commentShouldBeDeleted.emit(this.comment.id);
        },
        reject: () => {
            //reject action
        }
    });
}

  isAdmin() {
    return this.userService.hasRole(["ADMIN"]);
  }


}
