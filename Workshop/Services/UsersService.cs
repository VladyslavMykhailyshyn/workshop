using Workshop.Entities;
using Workshop.Repositories;

namespace Workshop.Services;

using AutoMapper;
using BCrypt.Net;

public interface IUsersService
{
    AuthenticateResponse Authenticate(AuthenticateRequest model);
    IEnumerable<User> GetAll();
    User GetById(string id);
    void Register(RegisterRequest model);
    void Update(string id, UpdateRequest model);
    void Delete(string id);
}

public class UsersService : IUsersService
{
    private IUsersRepository _usersRepository;
    private IJwtUtils _jwtUtils;
    private readonly IMapper _mapper;

    public UsersService(
        IUsersRepository usersRepository,
        IJwtUtils jwtUtils,
        IMapper mapper)
    {
        _usersRepository = usersRepository;
        _jwtUtils = jwtUtils;
        _mapper = mapper;
    }

    public AuthenticateResponse Authenticate(AuthenticateRequest model)
    {
        var user = _usersRepository.GetUserByUsername(model.Username);

        // validate
        if (user == null || !BCrypt.Verify(model.Password, user.PasswordHash))
            throw new AppException("Username or password is incorrect");

        // authentication successful
        var response = _mapper.Map<AuthenticateResponse>(user);
        response.Token = _jwtUtils.GenerateToken(user);
        return response;
    }

    public IEnumerable<User> GetAll()
    {
        return _usersRepository.GetAllUsers();
    }

    public User GetById(string id)
    {
        return getUser(id);
    }

    public void Register(RegisterRequest model)
    {
        // validate
        if (_usersRepository.GetUserByUsername(model.Username) is not null)
            throw new AppException("Username '" + model.Username + "' is already taken");

        // map model to new user object
        var user = _mapper.Map<User>(model);

        // hash password
        user.PasswordHash = BCrypt.HashPassword(model.Password);

        // save user
        _usersRepository.AddUser(user);
        //_context.SaveChanges();
    }

    public void Update(string id, UpdateRequest model)
    {
        var user = getUser(id);

        // validate
        if (model.Username != user.Username && _usersRepository.GetUserByUsername(model.Username) is not null)
            throw new AppException("Username '" + model.Username + "' is already taken");

        // hash password if it was entered
        if (!string.IsNullOrEmpty(model.Password))
            user.PasswordHash = BCrypt.HashPassword(model.Password);

        // copy model to user and save
        _mapper.Map(model, user);
        var userFromDb = _usersRepository.GetUser(id);
        userFromDb.Username = user.Username;
        userFromDb.PasswordHash = user.PasswordHash;
        //_context.SaveChanges();
    }

    public void Delete(string id)
    {
        var user = getUser(id);
        _usersRepository.DeleteUser(id);
    }

    // helper methods

    private User getUser(string id)
    {
        var user = _usersRepository.GetUser(id);
        if (user == null) throw new KeyNotFoundException("User not found");
        return user;
    }
}