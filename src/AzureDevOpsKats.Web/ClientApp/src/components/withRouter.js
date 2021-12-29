export const withRouter = (Component) => {
  const Wrapper = (props) => {
    
    return (
      <Component
        {...props}
        />
    );
  };
  
  return Wrapper;
};