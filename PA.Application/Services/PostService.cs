using AutoMapper;
using PA.Application.DTOs;
using PA.Application.Interfaces.Repositories;
using PA.Domain.Entities;
using PA.Domain.Enums;

namespace PA.Application.Services;

public class PostService
{
    private readonly IPostRepository _postRepository;
    private readonly IMapper _mapper;

    public PostService(IPostRepository postRepository, IMapper mapper)
    {
        _postRepository = postRepository;
        _mapper = mapper;
    }

    public async Task<PostDto?> GetByIdAsync(Guid id)
    {
        var post = await _postRepository.GetByIdAsync(id);
        return _mapper.Map<PostDto>(post);
    }

    public async Task<IEnumerable<PostDto>> GetRecentAsync(int count = 50)
    {
        var posts = await _postRepository.GetRecentPostsAsync(count);
        return _mapper.Map<IEnumerable<PostDto>>(posts);
    }

    public async Task<IEnumerable<PostDto>> GetPinnedAsync()
    {
        var posts = await _postRepository.GetPinnedPostsAsync();
        return _mapper.Map<IEnumerable<PostDto>>(posts);
    }

    public async Task<PostDto> CreateAsync(CreatePostDto dto, Guid authorId)
    {
        var postType = Enum.Parse<PostType>(dto.Type);
        var post = new Post(dto.Content, authorId, postType, dto.ImageUrl);

        var created = await _postRepository.AddAsync(post);
        return _mapper.Map<PostDto>(created);
    }

    public async Task UpdateAsync(Guid id, UpdatePostDto dto)
    {
        var post = await _postRepository.GetByIdAsync(id);
        if (post == null)
            throw new KeyNotFoundException("Post não encontrado");

        post.UpdateContent(dto.Content, dto.ImageUrl);
        await _postRepository.UpdateAsync(post);
    }

    public async Task DeleteAsync(Guid id)
    {
        await _postRepository.DeleteAsync(id);
    }

    public async Task PinAsync(Guid id)
    {
        var post = await _postRepository.GetByIdAsync(id);
        if (post == null)
            throw new KeyNotFoundException("Post não encontrado");

        post.Pin();
        await _postRepository.UpdateAsync(post);
    }

    public async Task UnpinAsync(Guid id)
    {
        var post = await _postRepository.GetByIdAsync(id);
        if (post == null)
            throw new KeyNotFoundException("Post não encontrado");

        post.Unpin();
        await _postRepository.UpdateAsync(post);
    }
}
