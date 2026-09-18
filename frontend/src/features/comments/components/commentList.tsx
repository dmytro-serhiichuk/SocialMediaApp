import Comment from "./comment";

export default function CommentList() {
    // TODO: load data from api and display via infinity scroll
    return (
        <div className="comments-list">
            <Comment />
            <Comment />
            <Comment />
            <Comment />
        </div>
    );
}