namespace PersistenceNet.MessagesProvider
{
    public abstract class Messages
    {
        public virtual string Error => "ERROR";
        public virtual string Warning => "WARNING";
        public virtual string Success => "SUCCESS";
        public virtual string Unknown => "[unknown]";

        public virtual string BadRequest => "Application exception occurred.";
        public virtual string NotFound => "The requested key was not found.";
        public virtual string Unauthorized => "Unauthorized access.";
        public virtual string InternalServerError => "Internal server error. Please try again later.";
        public virtual string UnexpectedOccurred => "An unexpected error occurred.";

        public virtual string EntityNull => "The entity is void";
        public virtual string RegisterSuccess => "Registration successfully completed!";
        public virtual string UpdateSuccess => "Upgrade successfully performed!";
        public virtual string DeleteSuccess => "Delete successfully performed!";
        public virtual string AddSuccess => "successfully added!";
        public virtual string IncludeProblem => "Ops, something went wrong by including the entity!";
        public virtual string UpdateProblem => "Ops, something went wrong updating the entity!";
        public virtual string DeleteProblem => "Ops, something went wrong deleting the entity!";
        public virtual string UnexpectedError => "An unexpected error occurred in the deletion of the entity";
        public virtual string CheckProperty => "Checks if property it's a first or foreign key.";
        public virtual string PropertyRequired => "required, but no message configured on the entity!";
        public virtual string EntityFound => "Entity not found!";

        public virtual string EntityConversion => "Starting the object to entity conversion in the method";
        public virtual string StartCallMethod => "Converting the object to the entity successfully performed in the method";

        public virtual string NoResultList => "No records found at this time with the filters provided!";
        public virtual string NoResult => "No records found!";

        public virtual string FilterMethod => "Starting the filter method in";
        public virtual string ConvertMethodFilter => "Converting the entity to the object successfully performed in the filter method.";

        public virtual string TransactionErrorUnexpected => "Unexpected error when completing transaction";
        public virtual string TransactionError => "Error initiating transaction!";
        public virtual string TransactionNoStarting => "The 'CommitAsync' action needs an active transaction. Try to start the method first 'BeginTransactionAsync'!";
    }
}